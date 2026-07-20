"use strict";
var login = true;
var token = null;
var pathBase = document.baseURI.replace(/\/$/, "");
// console.log(`pathBase: ${pathBase}`);
var connection = new signalR.HubConnectionBuilder().withUrl(pathBase+"/chatHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
    accessTokenFactory: () => token
}).configureLogging(signalR.LogLevel.Trace).build();
// Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
connection.on("ReceiveMessageFromUser", function (user, message) {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg = user + " says " + msg;
    let li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messageList").appendChild(li);
});
connection.on("ReceiveMessage", function (message) {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg = "[anonymous] says " + msg;
    let li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messageList").appendChild(li);
});
//connection.start().then(function (message) {
//    console.log("SignalR connection started successfully! "+message);
//    document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err);
//});

// Wait until the page loads
document.addEventListener("DOMContentLoaded", function () {
    let inputElement = document.getElementById("messageInput");
    // Listen for the keyup event
    inputElement.addEventListener("keyup", function (event) {
        // Get the current text typed in the box
        const currentText = inputElement.value.trim();
        //console.log(`keyup value: ${currentText}, length: ${currentText.length}`)
        document.getElementById("sendButton").disabled = currentText.length === 0;
    });
});
document.getElementById("sendButton").addEventListener("click", function (event) {
    const user = document.getElementById("username").value;
    const message = document.getElementById("messageInput").value;
    if (user)
        connection.invoke("ReceiveMessageFromUser", user, message).catch(function (err) { return console.error(err.toString()); });
    else
        connection.invoke("ReceiveMessage", message).catch(function (err) { return console.error(err.toString()); });
    document.getElementById("messageInput").value = "";
    document.getElementById("sendButton").disabled = true;
    event.preventDefault();
});
document.getElementById("loginButton").addEventListener("click", function (event) {
    const btn = document.getElementById("loginButton").value;
    if (btn === "Login") {
        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;
        if (username && password) {
            $.ajax({
                url: pathBase + "/api/auth/login",
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify({ UserName: username, Password: password, RememberMe: true }),
                processData: false,
                success: function (data) {
                    if (data.accessToken.token)
                    {
                        console.log("User "+username+" login successfully!");
                        token = data.accessToken.token;
                        connection.start().then(function (message) {
                            console.log("SignalR connection started successfully!");
                            const messageInput = document.getElementById("messageInput").value.trim();
                            document.getElementById("sendButton").disabled = messageInput.length === 0;
                            document.getElementById("loginButton").value = "Logout";
                        }).catch(function (err) {
                            return console.error("Invalid user! "+username+" "+err);
                        });
                    } else
                        console.error("Login failed to get a valid access token!");
                },
                error: function (data) {
                    console.error("Login failed! " + data.status + " " + data.statusText);
                    for (let i = 0; i < data.responseJSON.$values.length; i++)
                      console.error(data.responseJSON.$values[i].code + " " + data.responseJSON.$values[i].description);
                }
            });
        }
    } else if (btn === "Logout") {
        token = null;
        const messageInput = document.getElementById("messageInput").value.trim();
        document.getElementById("sendButton").disabled = messageInput.length === 0;
        connection.stop().then(function () {
            document.getElementById("loginButton").value = "Login";
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});