var https = require('http');
https.createServer(module.exports = function (callback, text, to) {
    const translate = require('google-translate-api');
    translate(text, { to: to }).then((res) => {
        if (res.from.language.iso === to) {
            callback(null, null);
        }
        else {
            callback(null, res.text);
        }
    }).catch((err) => {
        console.error(err);
    });
}).listen(9000, '127.0.0.8').listen(9000, 'localhost');
//# sourceMappingURL=translate.js.map