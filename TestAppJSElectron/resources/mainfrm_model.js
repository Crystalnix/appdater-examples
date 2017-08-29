
var button = document.querySelector("#CheckUpdates_Btn");
button.addEventListener("click", function() {
    CheckUpdates();
});

function CheckUpdates(){
    var exec = require('child_process').exec;
    exec('"C:\\Program Files (x86)\\Crystalnix\\Update\\1.3.99.0\\GoogleUpdate.exe" /machine /ua /installsource ondemand'); 
    console.log("click");
}