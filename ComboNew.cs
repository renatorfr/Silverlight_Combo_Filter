using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

public class ComboNew : ComboBox
{
    private ScrollViewer m_ScrollViewer;
    private Key m_OldKey;
    private Int32 m_Position;
    private IEnumerable<Object> m_HitList;

    public ComboNew()
        : base()
    {
        this.KeyDown += new KeyEventHandler(ComboNew_KeyDown);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        m_ScrollViewer = this.GetTemplateChild("ScrollViewer") as ScrollViewer;
        m_ScrollViewer.KeyDown += new KeyEventHandler(ComboNew_KeyDown);
    }

    void ComboNew_KeyDown(object sender, KeyEventArgs e)
    {
        if (this.Items.Count == 0) return;

        if (m_OldKey == e.Key && m_HitList != null && m_HitList.Count() > 0)
            m_Position = m_Position < m_HitList.Count() - 1 ? m_Position + 1 : 0;
        else
        {
            if (String.IsNullOrEmpty(this.DisplayMemberPath))
                m_HitList = from c in Items where c.ToString().StartsWith(e.Key.ToString(), StringComparison.CurrentCultureIgnoreCase) select c;
            else
                m_HitList = from c in Items where getStringValue(c).StartsWith(e.Key.ToString(), StringComparison.CurrentCultureIgnoreCase) select c;

            if (m_HitList.Any())
                m_Position = 0;
            else
                return;

            m_OldKey = e.Key;
        }

        var hit = m_HitList.ElementAt(m_Position);

        if (hit != null)
            SelectedItem = hit;
    }

    private String getStringValue(Object item)
    {
        System.Type itemType = item.GetType();
        PropertyInfo pi = itemType.GetProperty(DisplayMemberPath, BindingFlags.Public | BindingFlags.Instance);
        if (pi != null)
            return pi.GetValue(item, null).ToString();

        return null;
    }
}
