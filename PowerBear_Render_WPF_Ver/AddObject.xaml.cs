﻿using PowerBear_Render_WPF_Ver.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PowerBear_Render_WPF_Ver {
    /// <summary>
    /// AddObject.xaml 的交互逻辑
    /// </summary>
    public partial class AddObjectWindow : Window {
        public double angleY { get; set; } = 0;
        public AddObjectWindow() {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Button_Click_AddSphere(object sender, RoutedEventArgs e) {
            GobVar.fnObjects.Add(new NormalObject(new Box()));
        }
    }
}
