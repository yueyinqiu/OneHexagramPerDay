﻿using Microsoft.AspNetCore.Components;
using OneHexagramPerDayCore;
using System.Globalization;
using System;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;
using ChineseLunisolarCalendarYjFwkExtensions.Extensions;

namespace BlazorApp.Pages;

public partial class Index
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "p")]
    public string? PaintingToShowInString { get; set; }

    private string? hexagramName;
    private char? hexagramChar;
    private readonly GuaHexagram hexagramToday;
    private GuaHexagram? hexagramToShow;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(this.PaintingToShowInString))
            this.hexagramToShow = this.hexagramToday;
        else
            _ = GuaHexagram.TryParse(this.PaintingToShowInString, out this.hexagramToShow);

        if (this.hexagramToShow is not null && this.hexagramToShow.Count is 6)
            this.hexagramChar = this.hexagramToShow.ToUnicodeChar();
    }

    protected override async Task OnParametersSetAsync()
    {
        var zhouyi = await this.ZhouyiStoreProvider.GetZhouyiStoreAsync();

        if (this.hexagramToShow is not null && this.hexagramToShow.Count is 6)
        {
            var hex = zhouyi[this.hexagramToShow];

            var (upperPainting, lowerPainting) = hex.SplitToTrigrams();

            var upper = zhouyi.InnerStore.GetTrigram(upperPainting);
            var lower = zhouyi.InnerStore.GetTrigram(lowerPainting);

            this.hexagramName = upperPainting == lowerPainting ?
                $"{hex.Name}为{upper.Nature}" :
                $"{upper.Nature}{lower.Nature}{hex.Name}";
        }
    }

    private readonly DateOnly date;
    public Index()
    {
        var current = DateTime.Now;
        this.date = DateOnly.FromDateTime(current);
        this.hexagramToday = new HexagramProvider(this.date).GetHexagram();
    }

    private string DateTimeString
    {
        get
        {
            var dateTime = date.ToDateTime(new TimeOnly(6, 30));
            var calendar = new ChineseLunisolarCalendar();
            var nongliString =
                $"{calendar.GetYearGanzhiInChinese(dateTime)}年" +
                $"{calendar.GetMonthInChinese(dateTime)}月" +
                $"{calendar.GetDayInChinese(dateTime)}";

            return $"{nongliString} {dateTime:yyyy/MM/dd}";
        }
    }
}