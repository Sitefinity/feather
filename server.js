var connect = require('connect');
var port = '3336'
connect().use(connect.static(__dirname + '/')).listen(port);
//////////////////////////////////////////////////////
console.log("--------------------------------");
console.log("| Server started at port: " + port + ' |');
console.log("--------------------------------");
console.log("The website is now available at: http://localhost:" + port);
console.log("Vo(^_^)oV");