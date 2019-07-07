﻿using System;
using System.Windows;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using SintaxStuff;
using RomStuff;

namespace Sintaxinator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ErrorMsg.Content = "";
        }

        private void InputFileSelect_Click(object sender, RoutedEventArgs e)
        {
            String inputFilename = FileUtility.selectInputFile();
            InputFilename.Text = inputFilename;
            OutputFilename.Text = FileUtility.determineOutputFilename(inputFilename);
        }

        private void OutputFileSelect_Click(object sender, RoutedEventArgs e)
        {
            OutputFilename.Text = FileUtility.selectOutputFile();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SintaxFixer sintaxFixer = new SintaxFixer(InputFilename.Text, OutputFilename.Text);
                if (EnableBitFlip.IsChecked == true)
                {
                    if (BitFlipsAuto.IsChecked == true)
                    {
                        sintaxFixer.flipBits(true);
                    }
                    else if (BitFlipsManual.IsChecked == true)
                    {
                        sintaxFixer.flipBits(false, ManualBits.Text, int.Parse(FlipRepeat.Text));
                    }
                }
                if (EnableReorder.IsChecked == true)
                {
                    if (ReorderAuto.IsChecked == true)
                    {
                        sintaxFixer.reorder(false);
                    }
                    else if (ReorderBankNo.IsChecked == true)
                    {
                        sintaxFixer.reorder(true);
                    }
                }
                sintaxFixer.save();

                if (EnableHeaderFix.IsChecked == true)
                {
                    HeaderFixer headerfixer = new HeaderFixer(OutputFilename.Text, OutputFilename.Text);
                    headerfixer.headerFix(
                        (bool)EnableHeaderSize.IsChecked, 
                        (bool)EnableHeaderComp.IsChecked, 
                        (bool)EnableHeaderChecksum.IsChecked,
                        (bool)EnableHeaderType.IsChecked ? Byte.Parse(RomType.Text, System.Globalization.NumberStyles.HexNumber) : (byte?)null,
                        (bool)EnableHeaderRamsize.IsChecked ? Byte.Parse(RamSize.Text, System.Globalization.NumberStyles.HexNumber) : (byte?)null
                    );
                    headerfixer.save();
                }

                if (OpenEmu.IsChecked == true)
                {
                    System.Diagnostics.Process.Start(OutputFilename.Text);
                }
            }
            catch (Exception hmm)
            {
                PopulateErrorMessage(hmm);
            }

        }

        private void BankCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OverdumpDetector od = new OverdumpDetector(InputFilename.Text);
                MessageBox.Show(od.getSizeInfo());
            }
            catch (Exception hmm)
            {
                PopulateErrorMessage(hmm);
            }
        }

        private void ManualBits_GotFocus(object sender, RoutedEventArgs e)
        {
            BitFlipsManual.IsChecked = true;
        }

        private void RomType_GotFocus(object sender, RoutedEventArgs e)
        {
            EnableHeaderType.IsChecked = true;
        }

        private void RamSize_GotFocus(object sender, RoutedEventArgs e)
        {
            EnableHeaderRamsize.IsChecked = true;
        }

        private void PopulateErrorMessage(Exception e)
        {
            ErrorMsg.Content = "★ " + e.Message;
        }

        private void BBDTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BBDFixer bbdFixer = new BBDFixer(InputFilename.Text, OutputFilename.Text);
                bbdFixer.TestFix();
                bbdFixer.save();
            }
            catch (Exception hmm)
            {
                PopulateErrorMessage(hmm);
            }
        }

    }
}