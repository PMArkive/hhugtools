﻿using System;
using System.Windows;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using Sintaxinator.Fixers;
using Common.Utility;
using Common.Rom;

namespace Sintaxinator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool bbdMode = false;
        
        public MainWindow()
        {
            InitializeComponent();
            ErrorMsg.Content = "";
        }

        private void InputFileSelect_Click(object sender, RoutedEventArgs e)
        {
            String inputFilename = FileSelection.SelectInputFile();
            InputFilename.Text = inputFilename;
            OutputFilename.Text = FileSelection.DetermineOutputFilename(inputFilename);
        }

        private void OutputFileSelect_Click(object sender, RoutedEventArgs e)
        {
            OutputFilename.Text = FileSelection.SelectOutputFile();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bbdMode)
                {
                    BBDFixer bbdFixer = new BBDFixer(InputFilename.Text, OutputFilename.Text);
                    bbdFixer.TestFix();
                    bbdFixer.Save();
                    return;
                }
                
                SintaxFixer sintaxFixer = new SintaxFixer(InputFilename.Text, OutputFilename.Text);
                if (EnableReorder.IsChecked == true && ReorderAuto.IsChecked == true)
                {
                    sintaxFixer.reorder(false, byte.Parse(ReorderMode.Text, System.Globalization.NumberStyles.HexNumber));
                }
                if (EnableBitFlip.IsChecked == true)
                {
                    if (BitFlipsAuto.IsChecked == true)
                    {
                        sintaxFixer.flipBits(true);
                    }
                    else if (BitFlipsManual.IsChecked == true)
                    {
                        String manualBits = "0x" + ManualBits1.Text + "|0x" + ManualBits2.Text + "|0x" 
                                            + ManualBits3.Text + "|0x" + ManualBits4.Text;
                        sintaxFixer.flipBits(false, manualBits, int.Parse(FlipRepeat.Text));
                    }
                }
                if (EnableReorder.IsChecked == true && ReorderBankNo.IsChecked == true)
                {
                    sintaxFixer.reorder(true, null);
                }
                sintaxFixer.Save();

                if (EnableHeaderFix.IsChecked == true)
                {
                    HeaderFixer headerfixer = new HeaderFixer(OutputFilename.Text, OutputFilename.Text);
                    headerfixer.HeaderFix(
                        (bool)EnableHeaderSize.IsChecked, 
                        (bool)EnableHeaderComp.IsChecked, 
                        (bool)EnableHeaderChecksum.IsChecked,
                        (bool)EnableHeaderType.IsChecked ? Byte.Parse(RomType.Text, System.Globalization.NumberStyles.HexNumber) : (byte?)null,
                        (bool)EnableHeaderRamsize.IsChecked ? Byte.Parse(RamSize.Text, System.Globalization.NumberStyles.HexNumber) : (byte?)null
                    );
                    headerfixer.Save();
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
                if (!bbdMode)
                {
                    OperationsHeader.Content = "BBD mode";
                    bbdMode = true;
                    XorInstructions.Visibility = Visibility.Hidden;
                    XorControls.Visibility = Visibility.Hidden;
                    ReorderControls.Visibility = Visibility.Hidden;
                    HeaderControls.Visibility = Visibility.Hidden;
                }
                else
                {
                    OperationsHeader.Content = "Sintax mode";
                    bbdMode = false;
                    XorInstructions.Visibility = Visibility.Visible;
                    XorControls.Visibility = Visibility.Visible;
                    ReorderControls.Visibility = Visibility.Visible;
                    HeaderControls.Visibility = Visibility.Visible;
                }

            }
            catch (Exception hmm)
            {
                PopulateErrorMessage(hmm);
            }
        }

    }
}
