/* To avoid CSS expressions while still supporting IE 7 and IE 6, use this script */
/* The script tag referencing this file must be placed before the ending body tag. */

/* Use conditional comments in order to target IE 7 and older:
	<!--[if lt IE 8]><!-->
	<script src="ie7/ie7.js"></script>
	<!--<![endif]-->
*/

(function() {
	function addIcon(el, entity) {
		var html = el.innerHTML;
		el.innerHTML = '<span style="font-family: \'bibmono\'">' + entity + '</span>' + html;
	}
	var icons = {
		'bibmono-gscholar-mono': '&#xe900;',
		'bibmono-gpatents-mono': '&#xe901;',
		'bibmono-utal-mono': '&#xe902;',
		'bibmono-corfo-mono': '&#xe903;',
		'bibmono-conicyt-mono': '&#xe904;',
		'bibmono-udec-mono': '&#xe905;',
		'bibmono-ula-mono': '&#xe906;',
		'bibmono-subpesca-mono': '&#xe907;',
		'bibmono-uct-mono': '&#xe908;',
		'bibmono-uchile-mono': '&#xe909;',
		'bibmono-umag-mono': '&#xe90a;',
		'bibmono-uach-mono': '&#xe90b;',
		'bibmono-pucv-mono': '&#xe90c;',
		'bibmono-ucsc-mono': '&#xe90d;',
		'bibmono-puc-mono': '&#xe90e;',
		'bibmono-uv-mono': '&#xe90f;',
		'bibmono-ust-mono': '&#xe910;',
		'0': 0
		},
		els = document.getElementsByTagName('*'),
		i, c, el;
	for (i = 0; ; i += 1) {
		el = els[i];
		if(!el) {
			break;
		}
		c = el.className;
		c = c.match(/bibmono-[^\s'"]+/);
		if (c && icons[c[0]]) {
			addIcon(el, icons[c[0]]);
		}
	}
}());
