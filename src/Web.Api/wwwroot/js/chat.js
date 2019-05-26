"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
    accessTokenFactory: () => this.getToken()//document.getElementById("tokenInput").value
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
function getToken() {
    return document.getElementById("tokenInput").value
}