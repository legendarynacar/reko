/* 
 * Copyright (C) 1999-2010 John K�ll�n.
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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public class ProcedureDialogInteractor
    {
        protected ProcedureDialog dlg;

        private IProcessorArchitecture arch;
        private SerializedProcedure proc;


        public ProcedureDialogInteractor(IProcessorArchitecture arch, SerializedProcedure proc)
        {
            this.arch = arch;
            this.proc = proc;
            if (proc.Signature != null && proc.Signature.Arguments == null)
            {
                proc.Signature.Arguments = new SerializedArgument[0];
            }
        }

        public ProcedureDialog CreateDialog()
        {
            dlg = new ProcedureDialog();
            PopulateFields();
            dlg.ArgumentList.SelectedIndexChanged += new EventHandler(ArgumentList_SelectedIndexChanged);
            dlg.Signature.TextChanged += new EventHandler(Signature_TextChanged);
            return dlg;
        }

        private void PopulateFields()
        {
            dlg.ProcedureName.Text = proc.Name.Trim();
            if (proc.Signature != null)
            {
                dlg.Signature.Text = StringizeSignature(proc.Signature, proc.Name);
                PopulateSignatureFields(proc.Signature);
            }
        }

        private string StringizeSignature(SerializedSignature sig, string name)
        {
            StringBuilder sb = new StringBuilder();
            if (sig.ReturnValue == null)
                sb.Append("void");
            else
                sb.Append(sig.ReturnValue.Name);
            sb.Append(" ");
            sb.Append(name);
            sb.Append("(");
            string sep = "";
            foreach (var arg in sig.Arguments)
            {
                sb.Append(sep);
                sep = ", ";
                sb.Append(arg.Type);
                sb.Append(" ");
                sb.Append(arg.Name);
            }
            sb.Append(")");
            return sb.ToString();
        }

        private void PopulateSignatureFields(SerializedSignature sig)
        {
            if (sig.ReturnValue != null)
            {
                ListViewItem item = new ListViewItem("<Return value>"); 
                item.Tag = sig.ReturnValue;
                dlg.ArgumentList.Items.Add(item);
            }
        }

        public void ApplyChangesToProcedure(Procedure selectedProcedure)
        {
            //$TODO: actually apply changes!
        }

        private void EnableControls(bool signatureIsValid)
        {
            dlg.OkButton.Enabled = signatureIsValid;
            dlg.Signature.ForeColor = signatureIsValid ? SystemColors.WindowText : Color.Red;
        }

        protected void ArgumentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlg.ArgumentList.SelectedItems.Count != 0)
            {
                dlg.ArgumentProperties.SelectedObjects = new object[] {
                    dlg.ArgumentList.SelectedItems[0].Tag
                };
            }
            else
                dlg.ArgumentProperties.SelectedObjects = new object[0];
        }

        protected void Signature_TextChanged(object sender, EventArgs e)
        {
            var parser = new SignatureParser(arch);
            parser.Parse(dlg.Signature.Text);
            EnableControls(parser.IsValid);
        }


    }
}
