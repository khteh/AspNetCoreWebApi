"use strict";
var login = true;
var token = null;
console.log("window.location.origin: "+window.location.origin);
console.log("window.location: "+window.location);
console.log("document.baseURI: "+document.baseURI);
// Read the launchSettings.json file into the launch variable.
//var process = require('process');
//var launch = require('./Properties/launchSettings.json');
// Holds information about the hosting environment.
var environment = {
    // The names of the different environments.
    development: "Development",
    staging: "Staging",
    production: "Production",
    // Gets the current hosting environment the application is running under.
    current: function () { 
        return process.env.ASPNETCORE_ENVIRONMENT ||
            (launch && launch.profiles['IIS Express'].environmentVariables.ASPNETCORE_ENVIRONMENT) ||
            this.development;
    },
    // Are we running under the development environment.
    isDevelopment: function () { return this.current() === this.development; },
    // Are we running under the staging environment.
    isStaging: function () { return this.current() === this.staging; },
    // Are we running under the production environment.
    isProduction: function () { return this.current() === this.production; },
    pathBase: function() {
        console.log("Env: "+ this.current())
        if (this.isProduction())
            return document.baseURI + "apistarter/";
        else
            return document.baseURI;
    }
};
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
//connection.start().then(function (message) {
//    console.log("SignalR connection started successfully! "+message);
//    document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err);
//});
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("username").value;
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
                url: environment.pathBase() + "api/auth/login",
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
                            document.getElementById("sendButton").disabled = false;
                            document.getElementById("loginButton").value = "Logout";
                        }).catch(function (err) {
                            return console.error("Invalid user! "+username+" "+err);
                        });
                    } else
                        console.error("Login failed to get a valid access token!");
                },
                error: function (data) {
                    console.error("Login failed!" + JSON.stringify(data));
                }
            });
        }
    } else if (btn === "Logout") {
        token = null;
        document.getElementById("sendButton").disabled = true;
        connection.stop().then(function () {
            document.getElementById("loginButton").value = "Login";
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});
