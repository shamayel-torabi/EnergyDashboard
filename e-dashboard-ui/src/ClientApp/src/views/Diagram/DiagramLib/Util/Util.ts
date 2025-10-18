
export class Util {
  static max(array: any, byProperty: any) {
    return Util.find(array, byProperty, function (value1: number, value2: number) {
      return value1 >= value2;
    });
  }

  static min(array: any, byProperty: any) {
    return Util.find(array, byProperty, function (value1: number, value2: number) {
      return value1 < value2;
    });
  }

  static fill(array: { [x: string]: any; length: any; }, value: any) {
    var k = array.length;
    while (k--) {
      array[k] = value;
    }
    return array;
  }

  static find(array: any[], byProperty: string | number, condition: { (value1: any, value2: any): boolean; (value1: any, value2: any): boolean; (arg0: any, arg1: any): void; (arg0: any, arg1: any): void; }) {
    if (!array || array.length === 0) {
      return;
    }

    var i = array.length - 1,
      result = byProperty ? array[i][byProperty] : array[i];
    if (byProperty) {
      while (i--) {
        if (condition(array[i][byProperty], result)) {
          result = array[i][byProperty];
        }
      }
    }
    else {
      while (i--) {
        if (condition(array[i], result)) {
          result = array[i];
        }
      }
    }
    return result;
  }

  static type<T>(obj: T) {
    return (toString.call(obj) as string).slice(8, -1);
  }
}

