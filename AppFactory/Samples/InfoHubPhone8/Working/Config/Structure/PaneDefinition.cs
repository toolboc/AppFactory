using System.Windows.Controls;

namespace InfoHubPhone8.Config.Structure
{
    public class PaneDefinition
    {
        public string Header { get; set; }
        public int MaxWidth { get; set; }
        public Orientation Orientation { get; set; }
        public string AppBarStateName { get; set; }

        public PaneDefinition()
        {
            Orientation = Orientation.Vertical;
        }
    }
}