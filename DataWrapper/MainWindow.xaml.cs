using System;
using System.Reflection;
using System.Windows;

namespace DataWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataSchemaFactory mDataSchemaFactory;
        /// <summary>
        ///
        /// </summary>
        /// <param name="currentAssembly"></param>
        private void BuildDataSchemas(Assembly currentAssembly)
        {
            mDataSchemaFactory.ParseAssembly(currentAssembly);
        }

        public MainWindow()
        {
            mDataSchemaFactory = new DataSchemaFactory();

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            BuildDataSchemas(currentAssembly);

            mDataSchemaFactory.FixupKnownTypes();

            mDataSchemaFactory.DebugKnowTypes();
            InitializeComponent();
        }
    }
}
