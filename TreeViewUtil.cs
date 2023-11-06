using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDS_Dispatches {
    internal class TreeViewUtil {
        public static void treeBeginUpdate(TreeView tv) {
            if (tv.InvokeRequired) {
                tv.Invoke(
                (MethodInvoker)(
                        () => tv.BeginUpdate()
                    )
                );
            } else {
                tv.BeginUpdate();
            }
        }

        public static void treeEndUpdate(TreeView tv) {
            if (tv.InvokeRequired) {
                tv.Invoke(
                (MethodInvoker)(
                        () => tv.EndUpdate()
                    )
                );
            } else {
                tv.EndUpdate();
            }
        }

        public static void treeClear(TreeView tv) {
            if (tv.InvokeRequired) {
                tv.Invoke(
                (MethodInvoker)(
                        () => tv.Nodes.Clear()
                    )
                );
            } else {
                tv.Nodes.Clear();
            }
        }

        public static TreeNode treeAdd(TreeView tv, string node) {
            if (tv.InvokeRequired) {
                return (TreeNode)tv.Invoke(
                    new Func<Object>(() => tv.Nodes.Add(node))
                );
            } else {
                return tv.Nodes.Add(node);
            }
        }

        public static TreeNode treeNodeAdd(TreeView tv, TreeNode tn, string node) {
            if (tv.InvokeRequired) {
                return (TreeNode)tv.Invoke(
                    new Func<Object>(() => tn.Nodes.Add(node))
                );
            } else {
                return tn.Nodes.Add(node);
            }
        }

        public static void treeNodeSetForeColor(TreeView tv, TreeNode tn, System.Drawing.Color col) {
            if (tv.InvokeRequired) {
                tv.Invoke(
                (MethodInvoker)(
                        () => tn.ForeColor = col
                    )
                );
            } else {
                tn.ForeColor = col;
            }
        }

        public static void treeSelect(TreeView tv, TreeNode node) {
            if (tv.InvokeRequired) {
                tv.Invoke(
                (MethodInvoker)(
                        () => tv.SelectedNode = node
                    )
                );
            } else {
                tv.SelectedNode = node;
            }
        }

        public static void treeUnselect(TreeView tv) {
            if (tv.InvokeRequired) {
                tv.Invoke(
                (MethodInvoker)(
                        () => tv.SelectedNode = null
                    )
                );
            } else {
                tv.SelectedNode = null;
            }
        }
    }
}
