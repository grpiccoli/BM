var https = require('http');
https.createServer(module.exports = function (callback: any, text: string, to: string) {
	const translate = require('google-translate-api');
	
    translate(text, { to: to }).then((res: any) => {
        if (res.from.language.iso === to) {
            callback(null, null);
        } else {
            callback(null, res.text);
        }
	}).catch((err: any) => {
		console.error(err);
    });
}).listen(9000, '127.0.0.8').listen(9000, 'localhost');