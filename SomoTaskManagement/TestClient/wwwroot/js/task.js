const webSocket = new WebSocket('wss://localhost:7103/ws/countEvidence');

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