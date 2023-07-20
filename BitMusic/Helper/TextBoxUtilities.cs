﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace BitMusic.Helper;

public class TextBoxUtilities
{
    public static readonly DependencyProperty AlwaysScrollToEndProperty = DependencyProperty.RegisterAttached(
        "AlwaysScrollToEnd",
        typeof(bool),
        typeof(TextBoxUtilities),
        new PropertyMetadata(false, AlwaysScrollToEndChanged));

    private static void AlwaysScrollToEndChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            bool alwaysScrollToEnd = e.NewValue != null && (bool)e.NewValue;
            if (alwaysScrollToEnd)
            {
                tb.ScrollToEnd();
                tb.TextChanged += TextChanged;
            }
            else
            {
                tb.TextChanged -= TextChanged;
            }
        }
        else
        {
            throw new InvalidOperationException(
                "The attached AlwaysScrollToEnd property can only be applied to TextBox instances.");
        }
    }

    public static bool GetAlwaysScrollToEnd(TextBox textBox)
    {
        if (textBox == null)
        {
            throw new ArgumentNullException(nameof(textBox));
        }

        return (bool)textBox.GetValue(AlwaysScrollToEndProperty);
    }

    public static void SetAlwaysScrollToEnd(TextBox textBox, bool alwaysScrollToEnd)
    {
        if (textBox == null)
        {
            throw new ArgumentNullException(nameof(textBox));
        }

        textBox.SetValue(AlwaysScrollToEndProperty, alwaysScrollToEnd);
    }

    private static void TextChanged(object sender, TextChangedEventArgs e)
    {
        ((TextBox)sender).ScrollToEnd();
    }
}
