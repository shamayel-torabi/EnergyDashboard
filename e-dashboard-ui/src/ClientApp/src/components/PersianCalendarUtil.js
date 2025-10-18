import { HijriParser } from '@syncfusion/ej2-base';

export class PersianCalendarUtil {
    static hijriYear(date) {
        var hDate = HijriParser.getHijriDate(date);
        return hDate.year;
    }

    static firstDateOfMonth(date) {
        var hDate = HijriParser.getHijriDate(date);
        var gDate = HijriParser.toGregorian(hDate.year, hDate.month, 1);
        return gDate;
    }

    static gregorianDate(year, month, day) {
        return HijriParser.toGregorian(year, month, day);
    }

    static lastDateOfMonth(dt) {
        var hDate = HijriParser.getHijriDate(dt);
        var gDate = HijriParser.toGregorian(hDate.year, hDate.month, PersianCalendarUtil.getDaysInMonth(hDate.month, hDate.year));
        var finalGDate = new Date(gDate.getTime());
        new Date(finalGDate.setDate(finalGDate.getDate() + 1));
        var finalHDate = HijriParser.getHijriDate(finalGDate);
        if (hDate.month === finalHDate.month) {
            return finalGDate;
        }
        finalHDate = HijriParser.getHijriDate(gDate);
        if (hDate.month === finalHDate.month) {
            return gDate;
        }
        return new Date(gDate.setDate(gDate.getDate() - 1));
    }

    static hijriMonth(date) {
        var hijriDate = HijriParser.getHijriDate(date);
        return (hijriDate.month);
    }

    static hijriFirstDayOfMonth(year, month) {
        let date = HijriParser.toGregorian(year, month, 1);
        return date;
    }

    static hijriLastDayOfMonth(year, month) {
        let date = HijriParser.toGregorian(year, month, 15);
        return PersianCalendarUtil.lastDateOfMonth(date);
    }

    
    static isMonthStart(date) {
        var hijriDate = HijriParser.getHijriDate(date);
        return (hijriDate.date === 1);
    }

    static isLeapYear(year) {
        return ((((((year - ((year > 0) ? 474 : 473)) % 2820) + 474) + 38) * 682) % 2816) < 682;
    }

    static getDaysInMonth(month, year) {
        var length = 0;
        if (month < 6)
            length = 31;
        else if (month < 11)
            length = 30;
        else if (month === 11 && this.isLeapYear(year)) {
            length = 30;
        }
        else
            length = 29;
        return length;
    }
}

