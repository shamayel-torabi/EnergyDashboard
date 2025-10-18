import { Style } from '../Shapes/Style';
import { Color } from './Color';

export class SVGUtil {

  // static jsonTohtml(html: string, json: Object): string {
  //   html += "<ul>";
  //   for (var name in json) {
  //     var value = json[name];
  //     html += '<li>';
  //     html += '<span>' + name + ' : </span>';
  //     if (typeof value === 'object' && value !== null) {
  //       var h = "";
  //       html += SVGUtil.jsonTohtml(h, value);
  //     }
  //     else {
  //       html += '<span>' + value + '</span>';
  //     }

  //     html += '</li>'
  //   }

  //   html += '</ul>';

  //   return html;
  // }

  static getSvgColorString(prop: string, value: string) {
    if (!value) {
      return prop + ': none; ';
    }
    else {
      var color = new Color(value),
        str = prop + ': ' + color.toRgb() + '; ',
        opacity = color.getAlpha();
      if (opacity !== 1) {
        //change the color in rgb + opacity
        str += prop + '-opacity: ' + opacity.toString() + '; ';
      }
      return str;
    }
  }

  static getSvgStyles(style: Style) {
    var st: Array<string> = [];
    if (style.strokeStyle) {
      let stroke = SVGUtil.getSvgColorString('stroke', style.strokeStyle);
      let strokeWidth = style.strokeWidth ? style.strokeWidth : '0';
      let s = 'stroke-width: ' + strokeWidth + '; ';
      st.push(stroke);
      st.push(s);
    }

    if (style.fillStyle) {
      let fill = SVGUtil.getSvgColorString('fill', style.fillStyle);
      st.push(fill);
    }

    var opacity = typeof style.opacity !== 'undefined' ? style.opacity : '1';
    var o = 'opacity: ' + opacity + ';';
    st.push(o);
    return st.join('');
  }
}

