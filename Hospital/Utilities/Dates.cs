using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Utilities
{
    public static class Dates
    {
        /// <summary>
        /// compare two dates and return seconds count.
        /// </summary>
        /// <param name="newDate"></param>
        /// <param name="oldDate"></param>
        /// <returns></returns>
        public static int CompareDateFromSecond(DateTime newDate, DateTime oldDate)
        {


            /*TimeSpan res = newDate.Subtract(oldDate); 

            int seconds = Convert.ToInt32(res.TotalSeconds);
            return seconds;*/
            int newDateSecond = newDate.Year * 31556952
                                + newDate.Month * 2629800
                                + newDate.Day * 86400
                                + newDate.Hour * 3600
                                + newDate.Minute * 60
                                + newDate.Second;
            int oldDateSecond = oldDate.Year * 31556952
                                + oldDate.Month * 2629800
                                + oldDate.Day * 86400
                                + oldDate.Hour * 3600
                                + oldDate.Minute * 60
                                + oldDate.Second;
            /*if (newDate.Year == oldDate.Year 
                && newDate.Month == oldDate.Month
                && newDate.Day == oldDate.Day)
            {
                int newDateSecond = newDate.Hour * 3600
                                    + newDate.Minute * 60
                                    + newDate.Second;
                int oldDateSecond = oldDate.Hour * 3600
                                    + oldDate.Minute * 60
                                    + oldDate.Second;
            }*/
            int result = newDateSecond - oldDateSecond;
            return result;
        }

        public static string ToShamsi(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return (pc.GetYear(value) + "/" + pc.GetMonth(value)
                    + "/" + pc.GetDayOfMonth(value) + " " + pc.GetHour(value).ToString("00") + ":" + pc.GetMinute(value).ToString("00"));
        }
        public static string ToShamsiDate(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return (value.PersianDayOfWeek() + " " + pc.GetYear(value) + "/" + pc.GetMonth(value)
                    + "/" + pc.GetDayOfMonth(value));
        }
        public static string ToShamsiTime(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return (pc.GetHour(value).ToString("00") + ":" + pc.GetMinute(value).ToString("00"));
        }

        public static string PersianDayOfWeek(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return "شنبه";
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return "یکشنبه";
            }
            else if (date.DayOfWeek == DayOfWeek.Monday)
            {
                return "دوشنبه";
            }
            else if (date.DayOfWeek == DayOfWeek.Tuesday)
            {
                return "سه‌شنبه";
            }
            else if (date.DayOfWeek == DayOfWeek.Wednesday)
            {
                return "چهارشنبه";
            }
            else if (date.DayOfWeek == DayOfWeek.Thursday)
            {
                return "پنج‌شنبه";
            }
            else if (date.DayOfWeek == DayOfWeek.Friday)
            {
                return "جمعه";
            }

            return "???";
        }

        public static bool IsInSameDay(DateTime date1, DateTime date2)
        {
            if (date1.Year == date2.Year 
            &&  date1.Month == date2.Month
            && date1.Day == date2.Day)
            {
                return true;
            }

            return false;
        }
    }

}
