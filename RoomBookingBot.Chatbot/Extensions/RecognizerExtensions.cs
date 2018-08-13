﻿using Microsoft.Recognizers.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomBookingBot.Chatbot.Extensions
{
    public static class RecognizerExtensions
    {
        public static ResolutionList ParseRecognizer(this List<ModelResult> value)
        {
            var result = new ResolutionList();

            foreach (var modelResult in value)
            {
                var resolution = modelResult.Resolution;
                if (resolution["values"] is List<Dictionary<string, string>> resolutionValues)
                {
                    foreach (Dictionary<string, string> possibleTime in resolutionValues)
                    {
                        if (possibleTime.ContainsKey("type") && possibleTime["type"] == "time")
                        {
                            result.Add(new Resolution() { ResolutionType = Resolution.ResolutionTypes.Time, Time1 = TimeSpan.Parse(possibleTime["value"]) });
                        }
                        else if (possibleTime.ContainsKey("type") && possibleTime["type"] == "datetime")
                        {
                            var date1 = DateTime.Parse(possibleTime["value"]);
                            if (date1 > DateTime.Now)
                            {
                                result.Add(new Resolution() { ResolutionType = Resolution.ResolutionTypes.DateTime, Date1 = date1 });
                            }
                        }
                        else if (possibleTime.ContainsKey("type") && possibleTime["type"] == "date")
                        {
                            var date1 = DateTime.Parse(possibleTime["value"]);
                            if (date1 > DateTime.Now)
                            {
                                result.Add(new Resolution() { ResolutionType = Resolution.ResolutionTypes.DateTime, Date1 = date1 });
                            }
                        }
                        else if (possibleTime.ContainsKey("type") && possibleTime["type"] == "datetimerange")
                        {
                            var date1 = DateTime.Parse(possibleTime["start"]);
                            var date2 = DateTime.Parse(possibleTime["end"]);

                            if (date1 > DateTime.Now && date2 > DateTime.Now)
                            {
                                result.Add(new Resolution() { ResolutionType = Resolution.ResolutionTypes.DateTimeRange, Date1 = DateTime.Parse(possibleTime["start"]), Date2 = DateTime.Parse(possibleTime["end"]) });
                            }
                        }
                        else if (possibleTime.ContainsKey("type") && possibleTime["type"] == "timerange")
                        {
                            result.Add(new Resolution() { ResolutionType = Resolution.ResolutionTypes.TimeRange, Time1 = TimeSpan.Parse(possibleTime["start"]), Time2 = TimeSpan.Parse(possibleTime["end"]) });
                        }
                        else
                        {

                        }
                    }
                }
            }

            return result;
        }

        public class ResolutionList : List<Resolution>
        {
            public Resolution.ResolutionTypes ResolutionType
            {
                get
                {
                    return this.Count > 0 ? this.FirstOrDefault().ResolutionType : Resolution.ResolutionTypes.Unknown;
                }
            }

            public bool NeedsDisambiguation
            {
                get
                {
                    return this.Count > 1;
                }
            }
        }
    }

    public class Resolution
    {
        public enum ResolutionTypes
        {
            Time, DateTime, TimeRange, DateTimeRange, Unknown
        }

        public ResolutionTypes ResolutionType { get; set; }

        public TimeSpan? Time1 { get; set; }
        public TimeSpan? Time2 { get; set; }
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }
    }
}
