const interval = 5000;                  // the time interval the data will requested to the controller's action micro-service
let getFactoryLoop;
let factoryStarted = false;

$('#startBtn').click(function () {
    startFactory();
});

$('#stopBtn').click(function () {
    stopFactory();
});

// Calls the StartFactory Controller Action and Starts the Factory
function startFactory() {
    // Starting the factory
    if (!factoryStarted) {
        factoryStarted = true;
    }
    else {
        InitMonitorValues();
    }

    $.get('./StartFactory', function (data, status) {
        if (status == 'success') console.log('Factory started');
        else console.log('Factory start failed');
    });

    // Getting the stages values
    getFactoryLoop = setInterval(getFactoryData, interval);
}

// Calls the StopFactory Controller Action and Starts the Factory
function stopFactory() {
    clearInterval(getFactoryLoop);
    $.get('./StopFactory', function (data, status) {
        if (status == 'success') console.log('Factory stopped');
        else console.log('Factory stop failed');
    });
}

// Gets the serialized allowed data from CakeFactory object by calling the corresponding Micro-service
function getFactoryData() {
    $.get('./GetFactoryDataMS', function (data, status) {
        if (status == 'success') {
            $('.stageValue').eq(0).text(data.preparationNumber);
            $('.stageValue').eq(1).text(data.cuissonNumber);
            $('.stageValue').eq(2).text(data.emballageNumber);
            $('.stageValue').eq(3).text(data.readyNumber);
        }
    });
}

// Initializes the different monitored values
function InitMonitorValues() {
    $('.stageValue').text('0');
}