using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmptyMauiApp.Views;

namespace EmptyMauiApp.Models
{
    class ReminderModel
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan Time { get; set; }
        public string Message { get; set; }
        public bool IsEnabled { get; set; }
    }
}
