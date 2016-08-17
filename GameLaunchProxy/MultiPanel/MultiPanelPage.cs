// http://www.codeproject.com/Articles/37397/A-Multipanel-Control-in-C

// http://creativecommons.org/licenses/publicdomain/

// Copyright-Only Dedication* (based on United States law) or Public Domain Certification
//
//The person or persons who have associated work with this document (the "Dedicator" or "Certifier") hereby either (a) certifies that, to the best of his knowledge, the work of authorship identified is in the public domain of the country from which the work is published, or (b) hereby dedicates whatever copyright the dedicators holds in the work of authorship identified below (the "Work") to the public domain. A certifier, moreover, dedicates any copyright interest he may have in the associated work, and for these purposes, is described as a "dedicator" below.
//
//A certifier has taken reasonable steps to verify the copyright status of this work. Certifier recognizes that his good faith efforts may not shield him from liability if in fact the work certified is not in the public domain.
//
//Dedicator makes this dedication for the benefit of the public at large and to the detriment of the Dedicator's heirs and successors. Dedicator intends this dedication to be an overt act of relinquishment in perpetuity of all present and future rights under copyright law, whether vested or contingent, in the Work. Dedicator understands that such relinquishment of all rights includes the relinquishment of all rights to enforce (by lawsuit or otherwise) those copyrights in the Work.
//
//Dedicator recognizes that, once placed in the public domain, the Work may be freely reproduced, distributed, transmitted, used, modified, built upon, or otherwise exploited by anyone for any purpose, commercial or non-commercial, and in any way, including by methods that have not yet been invented or conceived.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Liron.Windows.Forms
{
    [Designer(typeof(Liron.Windows.Forms.Design.MultiPanelPageDesigner))]
    public class MultiPanelPage : ContainerControl
    {
        public MultiPanelPage()
        {
            base.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Overridden from <see cref="Panel"/>.
        /// </summary>
        /// <remarks>
        /// Since the <see cref="MultiPanelPage"/> exists only
        /// in the context of a <see cref="MultiPanelControl"/>,
        /// it makes sense to always have it fill the
        /// <see cref="MultiPanelControl"/>. Hence, this property
        /// will always return <see cref="DockStyle.Fill"/>
        /// regardless of how it is set.
        /// </remarks>
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = DockStyle.Fill;
            }
        }

        /// <summary>
        /// Only here so that it shows up in the property panel.
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Overriden from <see cref="Control"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="MultiPanelPage.ControlCollection"/>.
        /// </returns>
        protected override System.Windows.Forms.Control.ControlCollection CreateControlsInstance()
        {
            return new MultiPanelPage.ControlCollection(this);
        }

        #region Classes
        public new class ControlCollection : Control.ControlCollection
        {
            /// <summary>
            /// </summary>
            public ControlCollection(Control owner)
                : base(owner)
            {
                if (owner == null)
                    throw new ArgumentNullException("owner", "Tried to create a MultiPanelPage.ControlCollection with a null owner.");
                MultiPanelPage c = owner as MultiPanelPage;
                if (c == null)
                    throw new ArgumentException("Tried to create a MultiPanelPage.ControlCollection with a non-MultiPanelPage owner.", "owner");
            }

            /// <summary>
            /// </summary>
            public override void Add(Control value)
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Tried to add a null value to the MultiPanelPage.ControlCollection.");
                MultiPanelPage p = value as MultiPanelPage;
                if (p != null)
                    throw new ArgumentException("Tried to add a MultiPanelPage control to the MultiPanelPage.ControlCollection.", "value");
                base.Add(value);
            }
        }
        #endregion

    }
}
