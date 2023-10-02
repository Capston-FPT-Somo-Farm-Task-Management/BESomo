"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/taskHub").build();

$(function () {
	connection.start().then(function () {
		console.log('Connected to taskHub');

		InvokeProducts();

	}).catch(function (err) {
		return console.error(err.toString());
	});
});
 
function InvokeProducts() {
	debugger;
	connection.invoke("SendTasks").catch(function (err) {
		return console.error(err.toString());
	});

}

connection.on("ReceivedTask", function (tasks) {
	BindProductsToGrid(tasks);
});

function BindProductsToGrid(tasks) {
	$('#tblProduct tbody').empty();

	var tr;
	$.each(tasks, function (index, task) {
		tr = $('<tr/>');
		tr.append(`<td>${(index + 1)}</td>`);
		tr.append(`<td>${task.startDate}</td>`);
		tr.append(`<td>${task.endDate}</td>`);
		tr.append(`<td>${task.description}</td>`);
		tr.append(`<td>${task.priority}</td>`);
		tr.append(`<td>${task.receiverId}</td>`);
		tr.append(`<td>${task.fieldId}</td>`);
		tr.append(`<td>${task.taskTypeId}</td>`);
		tr.append(`<td>${task.memberId}</td>`);
		tr.append(`<td>${task.habitantId}</td>`);
		tr.append(`<td>${task.name}</td>`);
		tr.append(`<td>${task.status}</td>`);
		tr.append(`<td>${task.createDate}</td>`);
		tr.append(`<td>${task.iterations}</td>`);
		tr.append(`<td>${task.repeat}</td>`);
		$('#tblProduct').append(tr);
	});
}


var backgroundColors = [
	'rgba(255, 99, 132, 0.2)',
	'rgba(54, 162, 235, 0.2)',
	'rgba(255, 206, 86, 0.2)',
	'rgba(75, 192, 192, 0.2)',
	'rgba(153, 102, 255, 0.2)',
	'rgba(255, 159, 64, 0.2)'
];
var borderColors = [
	'rgba(255, 99, 132, 1)',
	'rgba(54, 162, 235, 1)',
	'rgba(255, 206, 86, 1)',
	'rgba(75, 192, 192, 1)',
	'rgba(153, 102, 255, 1)',
	'rgba(255, 159, 64, 1)'
];