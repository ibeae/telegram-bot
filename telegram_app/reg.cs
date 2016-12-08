using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace telegram_app
{
    class reg
    {

        // Save a value.
        public static void SaveSetting(string app_name, string name, object value)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            sub_key.SetValue(name, value);
        }

        // Get a value.
        public static object GetSetting(string app_name, string name, object default_value)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            return sub_key.GetValue(name, default_value);
        }
        public static string GetSetting(string app_name, string name)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software\\"+app_name);
            string pathName = (string)reg_key.GetValue(name);
             return (pathName == null) ? string.Empty : pathName;            
        }

        // Load all of the saved settings.
        public static void LoadAllSettings(string app_name, Form frm)
        {
            // Load form settings.
            frm.SetBounds(
                (int)GetSetting(app_name, "FormLeft", frm.Left),
                (int)GetSetting(app_name, "FormTop", frm.Top),
                (int)GetSetting(app_name, "FormWidth", frm.Width),
                (int)GetSetting(app_name, "FormHeight", frm.Height));
            frm.WindowState = (FormWindowState)GetSetting(app_name,
                "FormWindowState", frm.WindowState);

            // Load the controls' values.
            LoadChildSettings(app_name, frm);
        }

        // Load all child control settings.
        public static void LoadChildSettings(string app_name, Control parent)
        {
            try
            {
                foreach (Control child in parent.Controls)
                {
                    // Restore the child's value.
                    switch (child.GetType().Name)
                    {
                        case "TextBox":
                            child.Text = GetSetting(app_name, child.Name, child.Text).ToString();
                            break;
                        case "ComboBox":
                            ComboBox cb = child as ComboBox;
                            cb.SelectedValue = GetSetting(app_name, child.Name, child.Text).ToString();
                            break;
                        case "CheckBox":
                            CheckBox chk = child as CheckBox;
                            chk.Checked = bool.Parse(GetSetting(app_name,
                                child.Name, chk.Checked.ToString()).ToString());
                            break;
                            // Add other control types here.
                    }

                    // Recursively restore the child's children.
                    LoadChildSettings(app_name, child);
                }
            }
            catch { MessageBox.Show("error load saving..."); }
        }


        // Save all of the form's settings.
        public static void SaveAllSettings(string app_name, Form frm)
        {
            // Save form settings.
            SaveSetting(app_name, "FormWindowState", (int)frm.WindowState);
            if (frm.WindowState == FormWindowState.Normal)
            {
                // Save current bounds.
                SaveSetting(app_name, "FormLeft", frm.Left);
                SaveSetting(app_name, "FormTop", frm.Top);
                SaveSetting(app_name, "FormWidth", frm.Width);
                SaveSetting(app_name, "FormHeight", frm.Height);
            }
            else
            {
                // Save bounds when we're restored.
                SaveSetting(app_name, "FormLeft", frm.RestoreBounds.Left);
                SaveSetting(app_name, "FormTop", frm.RestoreBounds.Top);
                SaveSetting(app_name, "FormWidth", frm.RestoreBounds.Width);
                SaveSetting(app_name, "FormHeight", frm.RestoreBounds.Height);
            }

            // Save the controls' values.
            SaveChildSettings(app_name, frm);
        }

        // Save all child control settings.
        public static void SaveChildSettings(string app_name, Control parent)
        {
            try
            {
                foreach (Control child in parent.Controls)
                {
                    // Save the child's value.
                    switch (child.GetType().Name)
                    {
                        case "TextBox":
                            SaveSetting(app_name, child.Name, child.Text);
                            break;
                        case "ComboBox":
                            ComboBox cb = child as ComboBox;
                            if (cb.SelectedItem != null)
                            {
                                SaveSetting(app_name, child.Name, cb.SelectedValue.ToString());
                            }
                            break;
                        case "CheckBox":
                            CheckBox chk = child as CheckBox;
                            SaveSetting(app_name, child.Name, chk.Checked.ToString());
                            break;
                            // Add other control types here.
                    }

                    // Recursively save the child's children.
                    SaveChildSettings(app_name, child);
                }
            }
            catch { MessageBox.Show("error saving..."); }
        }
    }
}
