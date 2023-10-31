//"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/notifyHub").build();

//$(function () {
//	connection.start().then(function () {
//		console.log('Connected to taskHub');

//		InvokeProducts();

//	}).catch(function (err) {
//		return console.error(err.toString());
//	});
//});


//function InvokeProducts() {
//	//1debugger;
//	connection.invoke("SendNotify").catch(function (err) {
//		return console.error(err.toString());
//	});

//}

//connection.on("ReceivedList", function (tasks) {
//	BindProductsToGrid(tasks);
//});

//function BindProductsToGrid(tasks) {
//	$('#tblProduct tbody').empty();

//	var tr;
//	$.each(tasks, function (index, task) {
//		tr = $('<tr/>');
//		tr.append(`<td>${(index + 1)}</td>`);
//		tr.append(`<td>${task.message}</td>`);
//		tr.append(`<td>${task.messageType}</td>`);
//		tr.append(`<td>${task.notificationDateTime}</td>`);
//		tr.append(`<td>${task.isRead}</td>`);
//		$('#tblProduct').append(tr);
//	});
//}


//var backgroundColors = [
//	'rgba(255, 99, 132, 0.2)',
//	'rgba(54, 162, 235, 0.2)',
//	'rgba(255, 206, 86, 0.2)',
//	'rgba(75, 192, 192, 0.2)',
//	'rgba(153, 102, 255, 0.2)',
//	'rgba(255, 159, 64, 0.2)'
//];
//var borderColors = [
//	'rgba(255, 99, 132, 1)',
//	'rgba(54, 162, 235, 1)',
//	'rgba(255, 206, 86, 1)',
//	'rgba(75, 192, 192, 1)',
//	'rgba(153, 102, 255, 1)',
//	'rgba(255, 159, 64, 1)'
//];


const webSocket = new WebSocket('wss://localhost:7103/ws/notifications');



webSocket.onopen = (event) => {
    console.log('WebSocket is open and connected.');
};

webSocket.onmessage = (event) => {
    const notificationData = JSON.parse(event.data);
    console.log('Received notification:', notificationData);
};

webSocket.onclose = (event) => {
    if (event.wasClean) {
        console.log('WebSocket is closed cleanly, code: ' + event.code + ', reason: ' + event.reason);
    } else {
        console.error('WebSocket connection was closed abruptly.');
    }
};

webSocket.onerror = (error) => {
    console.error('WebSocket error:', error);
};
