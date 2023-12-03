using System.Drawing;
using System.Windows.Forms;

namespace DbExport.UI.Controls
{
    public partial class WizardPage : UserControl
    {
        private readonly LabelPair[] labelPairs;
        private int stage = 0;

        public WizardPage()
        {
            InitializeComponent();

            labelPairs = new[] {
                new LabelPair(lblStage1, lblStage1Name),
                new LabelPair(lblStage2, lblStage2Name),
                new LabelPair(lblStage3, lblStage3Name),
                new LabelPair(lblStage4, lblStage4Name),
                new LabelPair(lblStage5, lblStage5Name),
                new LabelPair(lblStage6, lblStage6Name),
                new LabelPair(lblStage7, lblStage7Name),
            };
        }

        protected int Stage
        {
            get => stage;
            set
            {
                if (value != stage)
                {
                    if (0 < stage && stage <= labelPairs.Length)
                        labelPairs[stage - 1].Color = Color.White;

                    stage = value;

                    if (0 < stage && stage <= labelPairs.Length)
                        labelPairs[stage - 1].Color = Color.Orange;
                }
            }
        }

        class LabelPair
        {
            public LabelPair(Label bullet, Label name)
            {
                Bullet = bullet;
                Name = name;
            }
            
            public Label Bullet { get; private set; }
            
            public Label Name { get; private set; }

            public Color Color
            {
                get => Bullet.BackColor;
                set
                {
                    Bullet.BackColor = value;
                    Name.ForeColor = value;
                }
            }
        }
    }
}