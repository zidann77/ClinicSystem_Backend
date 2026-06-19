    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace BackendClinicProject.GlobalClasses
    {
        /// <summary>
        /// CLASS: clsLogger
        /// PURPOSE: Centralized logging service for Windows Event Log integration (Clinic System)
        /// </summary>
        public static class clsLogger
        {
            // تغيير الاسم ليعبر عن السيستم الحالي
            const string sourceName = "ClinicSystemAPI";
            const string logName = "Application";

            /// <summary>
            /// بياخد رسالة عادية ونوع اللوج (مع لقط اسم الكلاس والميثود تلقائياً)
            /// </summary>
            public static void LogMessage(string message, EventLogEntryType type)
            {
                try
                {
                    // تأمين إنشاء الـ Source جوه الـ try-catch
                    if (!EventLog.SourceExists(sourceName))
                    {
                        EventLog.CreateEventSource(sourceName, logName);
                    }

                    // لقط اسم الكلاس والدالة اللي نادت اللوج
                    var stackTrace = new StackTrace();
                    var callingFrame = stackTrace.GetFrame(1);
                    var method = callingFrame?.GetMethod();
                    var className = method?.DeclaringType?.Name ?? "UnknownClass";
                    string methodName = method?.Name ?? "UnknownMethod";

                    string fullMessage = $"[Context: {className}.{methodName}]\nMessage: {message}\nTimestamp: {DateTime.Now}";

                    EventLog.WriteEntry(sourceName, fullMessage, type);
                }
                catch (Exception ex)
                {
                    // خطة بديلة لو الـ Event Log فشل (بسبب الصلاحيات مثلاً)
                    Console.WriteLine($"LOGGING FAILED: {ex.Message}");
                    Console.WriteLine($"ORIGINAL MESSAGE: {message}");
                }
            }

            /// <summary>
            /// OVERLOAD: ده الأفضل للاستخدام جوه الـ catch بالـ API لأنه بياخد الـ Exception كامل
            /// </summary>
            public static void LogException(Exception exception, string customMessage = "")
            {
                try
                {
                    if (!EventLog.SourceExists(sourceName))
                    {
                        EventLog.CreateEventSource(sourceName, logName);
                    }

                    // صياغة تفصيلية وممتازة للأخطاء البرمجية
                    string fullMessage = $"Custom Message: {customMessage}\n" +
                                         $"Exception: {exception.Message}\n" +
                                         $"Target Site: {exception.TargetSite}\n" +
                                         $"Stack Trace:\n{exception.StackTrace}\n" +
                                         $"Timestamp: {DateTime.Now}";

                    EventLog.WriteEntry(sourceName, fullMessage, EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LOGGING EXCEPTION FAILED: {ex.Message}");
                    Console.WriteLine($"ORIGINAL EXCEPTION: {exception.Message}");
                }
            }
        }
    }

