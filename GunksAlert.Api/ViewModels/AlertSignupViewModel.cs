using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using GunksAlert.Api.Models;

namespace GunksAlert.Api.ViewModels;

public class AlertSignupViewModel {
    // [Required]
    // public required Crag Crag { get; set; }

    // [Required]
    // public required ClimbableConditions Conditions { get; set; }

    public bool Sunday { get; set; } = false;
    public bool Monday { get; set; } = false;
    public bool Tuesday { get; set; } = false;
    public bool Wednesday { get; set; } = false;
    public bool Thursday { get; set; } = false;
    public bool Friday { get; set; } = false;
    public bool Saturday { get; set; } = false;

    public bool January { get; set; } = false;
    public bool February { get; set; } = false;
    public bool March { get; set; } = false;
    public bool April { get; set; } = false;
    public bool May { get; set; } = false;
    public bool June { get; set; } = false;
    public bool July { get; set; } = false;
    public bool August { get; set; } = false;
    public bool September { get; set; } = false;
    public bool October { get; set; } = false;
    public bool November { get; set; } = false;
    public bool December { get; set; } = false;

    // Not elegant. But simple ¯\_(ツ)_/¯
    public List<string> GetWeekdayNames() {
        List<string> days = [];
        if (Sunday) days.Add("Sunday");
        if (Monday) days.Add("Monday");
        if (Tuesday) days.Add("Tuesday");
        if (Wednesday) days.Add("Wednesday");
        if (Thursday) days.Add("Thursday");
        if (Friday) days.Add("Friday");
        if (Saturday) days.Add("Saturday");

        return days;
    }

    public List<string> GetMonthNames() {
        List<string> months = [];
        if (January) months.Add("January");
        if (February) months.Add("February");
        if (March) months.Add("March");
        if (May) months.Add("May");
        if (May) months.Add("May");
        if (June) months.Add("June");
        if (July) months.Add("July");
        if (August) months.Add("August");
        if (September) months.Add("September");
        if (October) months.Add("October");
        if (November) months.Add("November");
        if (December) months.Add("December");

        return months;
    }
}
