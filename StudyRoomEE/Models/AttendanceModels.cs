namespace StudyRoomEE.Models
{
using System;
using System.Collections.Generic;
using System.Linq;

public class AttendanceLog
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Status { get; set; } = ""; // 入室, 一時退出, 一時退出戻り, 退室
        public DateTime Timestamp { get; set; }
    }

    public class StudyTimeResult
    {
        public TimeSpan TotalAwayTime { get; set; } // 一時退出合計
        public TimeSpan NetStudyTime { get; set; }  // 純自習時間
    }

    public static class StudyCalculator
    {
        public static StudyTimeResult Calculate(IEnumerable<AttendanceLog> logs)
        {
            // 1. 計算前に、すべてのログのタイムスタンプからミリ秒を切り捨てる
            var sortedLogs = logs.Select(l => new AttendanceLog
            {
                Status = l.Status,
                Timestamp = new DateTime(l.Timestamp.Year, l.Timestamp.Month, l.Timestamp.Day,
                                         l.Timestamp.Hour, l.Timestamp.Minute, l.Timestamp.Second)
            })
            .OrderBy(l => l.Timestamp).ToList();

            TimeSpan away = TimeSpan.Zero;
            TimeSpan net = TimeSpan.Zero;
            DateTime? lastInTime = null;
            DateTime? lastAwayTime = null;

            foreach (var log in sortedLogs)
            {
                switch (log.Status)
                {
                    case "入室":
                    case "一時退出戻り":
                        lastInTime = log.Timestamp;
                        if (lastAwayTime.HasValue)
                        {
                            away += log.Timestamp - lastAwayTime.Value;
                            lastAwayTime = null;
                        }
                        break;
                    case "一時退出":
                        if (lastInTime.HasValue)
                        {
                            net += log.Timestamp - lastInTime.Value;
                            lastInTime = null;
                        }
                        lastAwayTime = log.Timestamp;
                        break;
                    case "退室":
                        if (lastInTime.HasValue)
                        {
                            net += log.Timestamp - lastInTime.Value;
                            lastInTime = null;
                        }
                        break;
                }
            }
            return new StudyTimeResult { TotalAwayTime = away, NetStudyTime = net };
        }
    }
}
