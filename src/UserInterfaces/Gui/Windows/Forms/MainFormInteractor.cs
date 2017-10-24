﻿#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Gui.Forms;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace Reko.Gui.Windows.Forms
{
    public class MainFormInteractor : Reko.Gui.Forms.MainFormInteractor
    {
        public MainFormInteractor(IServiceProvider services) : base(services)
        {
        }

        public override IMainForm LoadForm()
        {
            var form = base.LoadForm();
            form.ProcessCommandKey += this.MainForm_ProcessCommandKey;
            return form;
        }

        public override void Run()
        {
            Application.Run((Form)LoadForm());
        }

        private void MainForm_ProcessCommandKey(object sender, KeyEventArgs e)
        {
            dm.ProcessKey(uiSvc, e);
        }

        protected override void UpdateToolbarState(IMainForm form)
        {
            var status = new CommandStatus();
            var text = new CommandText();
            foreach (ToolStripItem item in form.ToolBar.Items)
            {
                var cmd = item.Tag as MenuCommand;
                if (cmd != null)
                {
                    text.Text = null;
                    var st = QueryStatus(cmd.CommandID, status, text);
                    item.Enabled = st && (status.Status & MenuStatus.Enabled) != 0;
                    if (!string.IsNullOrEmpty(text.Text))
                        item.Text = text.Text;
                }
            }
        }

    }
}
