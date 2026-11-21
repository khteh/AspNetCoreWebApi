"use strict";
// Requires @microsoft/signalr 11.x (authentication refresh support).
var pathBase = document.baseURI.replace(/\/$/, "");

// ---- Token store -----------------------------------------------------------
// The access token is a 2-hour JWT (JwtIssuerOptions.ValidFor) validated with
// ClockSkew = 0. The refresh token is exchanged at POST /api/auth/refreshtoken.
const REFRESH_BEFORE_EXPIRY_MS = 5 * 60 * 1000; // SignalR refreshes 5 minutes before expiry
const RENEW_MARGIN_MS = REFRESH_BEFORE_EXPIRY_MS + 60 * 1000; // renew a little earlier than that

const auth = {
    accessToken: null,
    refreshToken: null,
    expiresAt: 0,        // local clock, so client/server clock drift doesn't matter
    pending: null        // de-duplicates concurrent exchanges
};

function storeTokens(data) {
    auth.accessToken = data.accessToken.token;
    auth.expiresAt = Date.now() + data.accessToken.expiresIn * 1000;
    if (data.refreshToken)
        auth.refreshToken = data.refreshToken;
}

function clearTokens() {
    auth.accessToken = null;
    auth.refreshToken = null;
    auth.expiresAt = 0;
    auth.pending = null;
}

async function exchangeRefreshToken() {
    const response = await fetch(pathBase + "/api/auth/refreshtoken", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ AccessToken: auth.accessToken, RefreshToken: auth.refreshToken })
    });
    const data = await response.json().catch(() => null);
    if (!response.ok || !data || !data.accessToken || !data.accessToken.token)
        throw new Error("Refresh token exchange failed: " + response.status);
    storeTokens(data);
    return auth.accessToken;
}

// SignalR calls this before negotiate, before opening the WebSocket, and on
// every authentication refresh. It must return a token that is still valid,
// and on refresh it must return a NEW token, otherwise the server reports the
// same short remaining lifetime and nothing is gained.
async function getAccessToken() {
    if (!auth.accessToken)
        return null; // not logged in; the hub's ApiUser policy will reject the connection
    if (Date.now() < auth.expiresAt - RENEW_MARGIN_MS)
        return auth.accessToken;
    if (!auth.pending)
        auth.pending = exchangeRefreshToken().finally(() => { auth.pending = null; });
    return auth.pending;
}

// ---- Connection ------------------------------------------------------------
// skipNegotiation must be false: the refresh feature is only configured from the
// negotiate response (connection token + tokenLifetimeSeconds). The server only
// allows WebSockets, so keep that transport.
var connection = new signalR.HubConnectionBuilder()
    .withUrl(pathBase + "/chatHub", {
        transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: getAccessToken
    })
    .withAuthenticationRefresh({ refreshBeforeExpirationInMilliseconds: REFRESH_BEFORE_EXPIRY_MS })
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.onAuthenticationRefreshed(function (context) {
    console.log("SignalR authentication refreshed. New lifetime: " + context.newTokenLifetimeInSeconds + "s");
});
connection.onAuthenticationRefreshFailed(function (context) {
    // Not retried automatically. The server closes the connection at expiry
    // (CloseOnAuthenticationExpiration = true), and onclose resets the UI.
    console.error("SignalR authentication refresh failed: " + context.error);
});
connection.onclose(function (error) {
    if (error)
        console.error("SignalR connection closed: " + error);
    setLoggedOutUi();
});

// ---- UI --------------------------------------------------------------------
function appendMessage(text) {
    let li = document.createElement("li");
    li.textContent = text; // textContent already escapes HTML
    document.getElementById("messageList").appendChild(li);
}
function updateSendButton() {
    const text = document.getElementById("messageInput").value.trim();
    document.getElementById("sendButton").disabled =
        text.length === 0 || connection.state !== signalR.HubConnectionState.Connected;
}
function setLoggedOutUi() {
    document.getElementById("loginButton").value = "Login";
    updateSendButton();
}

document.getElementById("sendButton").disabled = true;
connection.on("ReceiveMessageFromUser", function (user, message) { appendMessage(user + " says " + message); });
connection.on("ReceiveMessage", function (message) { appendMessage("[anonymous] says " + message); });

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("messageInput").addEventListener("keyup", updateSendButton);
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();
    const user = document.getElementById("username").value;
    const message = document.getElementById("messageInput").value;
    const call = user
        ? connection.invoke("ReceiveMessageFromUser", user, message)
        : connection.invoke("ReceiveMessage", message);
    call.catch(function (err) { console.error(err.toString()); });
    document.getElementById("messageInput").value = "";
    updateSendButton();
});

document.getElementById("loginButton").addEventListener("click", async function (event) {
    event.preventDefault();
    const btn = document.getElementById("loginButton");
    if (btn.value === "Login") {
        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;
        if (!username || !password)
            return;
        try {
            const response = await fetch(pathBase + "/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ UserName: username, Password: password, RememberMe: true })
            });
            const data = await response.json().catch(() => null);
            if (!response.ok || !data || !data.accessToken || !data.accessToken.token) {
                console.error("Login failed! " + response.status + " " + response.statusText);
                const errors = data && (data.$values || (data.errors && (data.errors.$values || data.errors)));
                if (Array.isArray(errors))
                    errors.forEach(e => console.error(e.code + " " + e.description));
                return;
            }
            storeTokens(data);
            console.log("User " + username + " logged in successfully!");
            await connection.start();
            console.log("SignalR connection started successfully!");
            btn.value = "Logout";
            updateSendButton();
        } catch (err) {
            console.error("Unable to connect as " + username + ": " + err);
            clearTokens();
        }
    } else if (btn.value === "Logout") {
        clearTokens();
        try {
            await connection.stop(); // onclose resets the UI
        } catch (err) {
            console.error(err.toString());
        }
    }
});
