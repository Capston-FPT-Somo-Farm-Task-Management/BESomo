
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.start().then(function () {
    //debugger;
    console.log('connected to hub');
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("OnConnected", function () {
    OnConnected();
});

function OnConnected() {
    var username = $('#hfUserName').val();
    connection.invoke("SaveUserConnection", username).catch(function (err) {
        return console.error(err.toString());
    })
}

connection.on("ReceivedNotification", function (message) {
    DisplayGeneralNotification(message, 'General Message');
});


connection.on("ReceivedPersonalNotification", function (message, userName) {
    DisplayPersonalNotification(message, 'Hey ' + userName);
});

