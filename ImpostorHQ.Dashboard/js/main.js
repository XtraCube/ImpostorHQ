"use strict";
var connection = null;
var y = null;
var x = null;

function connect() {
	var serverUrl;
	var scheme = "ws";

	if (document.location.protocol === "https:") {
		scheme += "s";
	}
	
	serverUrl = scheme + "://" + document.location.hostname + ":22023";

	connection = new WebSocket(serverUrl);
	console.log("***CREATED WEBSOCKET");
  
	connection.onopen = function(evt) {
		console.log("***ONOPEN");
		document.getElementById("status").innerHTML = "";
		var auth = {
			Text: document.getElementById("apikey").value,
			Type: MessageFlags.LoginApiRequest,
			Date: Date.now()
		};
		connection.send(JSON.stringify(auth));
	};
	console.log("***CREATED ONOPEN");

	connection.onerror = function(event) {
		console.error("WebSocket error observed: ", event);
		document.getElementById("status").innerHTML = "WebSocket Error: " + event.type;
		document.getElementById("text").value = "";
		document.getElementById("text").disabled = true;
		document.getElementById("send").disabled = true;
	};
	console.log("***CREATED ONERROR");

	connection.onmessage = function(evt) {
		console.log("***ONMESSAGE");
		var box = document.getElementById("chatbox");
		var text = "";
		var msg = JSON.parse(evt.data);
		console.log("Message received: ");
		console.dir(msg);
		var time = new Date(msg.Date);
		var timeStr = time.toLocaleTimeString();

		switch(msg.Type) {
			case MessageFlags.ConsoleLogMessage:
				text = "(" + timeStr + ") [" + msg.Name + "] : " + msg.Text + "\n";
				break;
			case MessageFlags.LoginApiAccepted:
				document.getElementById("status").style.color = "green";
				document.getElementById("status").innerHTML = "Logged in!";
				document.getElementById("text").value = "";
				document.getElementById("text").disabled = false;
				document.getElementById("send").disabled = false;
				console.log("AUTHED");
			break;

			case MessageFlags.LoginApiRejected:
				document.getElementById("status").style.color = "red";
				document.getElementById("status").innerHTML = "Api Key Error!";
				document.getElementById("text").disabled = true;
				document.getElementById("send").disabled = true;
			break;

			case MessageFlags.DoKickOrDisconnect:
				document.getElementById("status").style.color = "red";
				document.getElementById("status").innerHTML = "Kicked:" + msg.Text;
				document.getElementById("text").value = "";
				document.getElementById("text").disabled = true;
				document.getElementById("send").disabled = true;
			break;

			case MessageFlags.HeartbeatMessage:
				var tokens = msg.Text.split("-");
				document.getElementById("Lobbies").innerHTML = tokens[0];
				document.getElementById("Players").innerHTML = tokens[1];
				console.log("HEARTBEAT")
			break;

			//	commented out for now, but could be used to transmit game room list
			//      case "userlist":
			//        var ul = "";
			//        var i;
			//
			//        for (i=0; i < msg.users.length; i++) {
			//          ul += msg.users[i] + "<br>";
			//        }
			//        document.getElementById("userlistbox").innerHTML = ul;
			//        break;
		}

		if (text.length) {
			  box.value += text; 
			  box.scrollTop = box.scrollHeight; 
		}
	};
	console.log("***CREATED ONMESSAGE");

}

function send() {
	console.log("***SEND");
	var msg = {
		Text: document.getElementById("text").value,
		Type: MessageFlags.ConsoleCommand,
		Date: Date.now()
	};
	connection.send(JSON.stringify(msg));
	document.getElementById("text").value = "";
}

function handleKey(evt) {
	if (evt.keyCode === 13 || evt.keyCode === 14) {
		if (!document.getElementById("send").disabled) {
		send();
		}
	}
}
const MessageFlags = 
{
	LoginApiRequest : "0",      // A request to log in, with a given API key.
    LoginApiAccepted : "1",     // The API Key is correct, so the login is successful.
    LoginApiRejected : "2",     // The API key is incorrect, so the login is rejected.
    ConsoleLogMessage : "3",    // Server Message
    ConsoleCommand : "4",       // A command sent from the dashboard to the API.
    HeartbeatMessage : "5",     // Quick sanity check with some statistics
    GameListMessage : "6",      // Not implemented yet.
    DoKickOrDisconnect : "7"    // A message when a client is kicked or the server shuts down.
}