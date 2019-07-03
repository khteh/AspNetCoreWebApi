"use strict";
var login = true;
var token = null;
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
    accessTokenFactory: () => token
}).configureLogging(signalR.LogLevel.Trace).build();
// Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
connection.on("ReceiveMessageFromUser", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messageList").appendChild(li);
});
connection.on("ReceiveMessage", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "[anonymous] says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messageList").appendChild(li);
});
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    if (user)
        connection.invoke("ReceiveMessageFromUser", user, message).catch(function (err) { return console.error(err.toString()); });
    else
        connection.invoke("ReceiveMessage", message).catch(function (err) { return console.error(err.toString()); });
    event.preventDefault();
});
document.getElementById("loginButton").addEventListener("click", function (event) {
    var btn = document.getElementById("loginButton").value;
    if (btn === "Login") {
        var username = document.getElementById("username").value;
        var password = document.getElementById("password").value;
        if (username && password) {
            $.ajax({
                url: "/api/auth/login",
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: { UserName: username, Password: password, RememberMe: true },
                processData: false,
                success: function (blob) {
                    console.log(data);
                    token = data.accessToken.token;
                },
                error: function (data) {
                    console.error("Login failed!" + JSON.stringify(data));
                }
            });
        }
    } else if (btn === "Logout") {
        this.token = null;
        document.getElementById("sendButton").disabled = true;
        connection.stop().then(function () {
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});
