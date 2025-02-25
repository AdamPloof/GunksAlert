using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Buffers.Binary;
using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Api.Models;

/// <summary>
/// Represents the date range for which alerts should be monitored and sent.
/// </summary>
/// <remarks>
/// Months and DaysOfWeek are stored as an integer which is converted to a bit mask that represents
/// the months of the year and days of the week. This allows for storing a collection of months and 
/// days of the week in a single row. This isn't the best option if normalization and easy querying
/// is the priority, but it was fun to implmement it this way so :P
/// </remarks>
public class AlertPeriod {
    [Key]
    public int Id { get; private set; }

    [Required]
    public DateTimeOffset StartDate { get; set; }

    [Required]
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Bit mask of the months this period targets.
    /// 
    /// Example: [1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1]
    /// translates to: ["January", "March", "December"]
    /// </summary>
    [Required]
    public int Months { get; private set; }

    /// <summary>
    /// Bit mask of the days of the week this period targets.
    /// 
    /// Example: [1, 1, 0, 0, 0, 0, 1]
    /// translates to: ["Sunday", "Friday", "Saturday"]
    /// </summary>
    [Required]
    public int DaysOfWeek { get; private set; }

    // **Month operations**
    public List<string> GetMonths() {
        string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
        List<string> months = new List<string>();
        int normalizedMonths = BitConverter.IsLittleEndian ? Months : BinaryPrimitives.ReverseEndianness(Months);
        BitArray monthBitMask = new BitArray(new int[] { normalizedMonths });
        for (int i = 0; i < 12; i++) {
            if (monthBitMask[i]) {
                months.Add(monthNames[i]);
            }
        }

        return months;
    }

    public void SetMonths(List<string> months) {
        string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
        HashSet<string> monthSet = new HashSet<string>(months);
        BitArray monthBitMask = new BitArray(32);
        for (int i = 0; i < 12; i++) {
            if (monthSet.Contains(monthNames[i])) {
                monthBitMask[i] = true;
            }
        }

        int[] intArr = new int[1];
        monthBitMask.CopyTo(intArr, 0);
        Months = BitConverter.IsLittleEndian ? intArr[0] : BinaryPrimitives.ReverseEndianness(intArr[0]);
    }

    public void AddMonth(string month) {
        List<string> monthNames = GetMonths();
        if (monthNames.Contains(month)) {
            return;
        }

        monthNames.Add(month);
        SetMonths(monthNames);
    }

    public void RemoveMonth(string month) {
        List<string> monthNames = GetMonths();
        if (!monthNames.Remove(month)) {
            return;
        }

        SetMonths(monthNames);
    }

    // **DaysOfWeek operations**
    public List<string> GetDaysOfWeek() {
        string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
        List<string> days = new List<string>();
        int normalizedDays = BitConverter.IsLittleEndian ? DaysOfWeek : BinaryPrimitives.ReverseEndianness(DaysOfWeek);
        BitArray dayBitMask = new BitArray(new int[] { normalizedDays });
        for (int i = 0; i < 7; i++) {
            if (dayBitMask[i]) {
                days.Add(dayNames[i]);
            }
        }

        return days;
    }

    public void SetDaysOfWeek(List<string> days) {
        string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
        HashSet<string> daySet = new HashSet<string>(days);
        BitArray dayBitMask = new BitArray(32);
        for (int i = 0; i < 7; i++) {
            if (daySet.Contains(dayNames[i])) {
                dayBitMask[i] = true;
            }
        }

        int[] intArr = new int[1];
        dayBitMask.CopyTo(intArr, 0);
        DaysOfWeek = BitConverter.IsLittleEndian ? intArr[0] : BinaryPrimitives.ReverseEndianness(intArr[0]);
    }

    public void AddDayOfWeek(string day) {
        List<string> dayNames = GetDaysOfWeek();
        if (dayNames.Contains(day)) {
            return;
        }

        dayNames.Add(day);
        SetDaysOfWeek(dayNames);
    }

    public void RemoveDayOfWeek(string day) {
        List<string> dayNames = GetDaysOfWeek();
        if (!dayNames.Remove(day)) {
            return;
        }

        SetDaysOfWeek(dayNames);
    }
}
