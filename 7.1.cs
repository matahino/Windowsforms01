using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Tt
{
    public class Form1 : Form
    {
        private Button myButton;
        private TextBox myTextBox;
        private ListBox myListBox;
        private Button mySortButton;
        private Button myDeleteButton;
        private string path;

        public Form1()
        {
            myButton = new Button
            {
                Size = new Size(100, 50),
                Location = new Point(200, 400),
                Text = "Click Me"
            };
            myButton.Click += new EventHandler(MyButton_Click);

            mySortButton = new Button
            {
                Size = new Size(100, 50),
                Location = new Point(300, 400),
                Text = "Sort Me"
            };
            mySortButton.Click += new EventHandler(MySortButton_Click);

            myDeleteButton = new Button
            {
                Size = new Size(100, 50),
                Location = new Point(400, 400),
                Text = "Delete"
            };
            myDeleteButton.Click += new EventHandler(MyDeleteButton_Click);

            myTextBox = new TextBox
            {
                Size = new Size(300, 40),
                Location = new Point(100, 50)
            };

            myListBox = new ListBox
            {
                Size = new Size(300, 300),
                Location = new Point(100, 100)
            };
            myListBox.MouseDoubleClick += new MouseEventHandler(MyListBox_MouseDoubleClick);

            Text = "Folder Viewer";
            ClientSize = new Size(600, 500);
            Controls.AddRange(new Control[] { myButton, mySortButton, myDeleteButton, myTextBox, myListBox });
        }

        private void MyButton_Click(object sender, EventArgs e)
        {
            path = myTextBox.Text;
            if (Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                myListBox.Items.Clear();
                foreach (string directory in directories)
                {
                    myListBox.Items.Add(new DirectoryInfo(directory).Name);
                }
            }
            else
            {
                MessageBox.Show("Invalid path!");
            }
        }

        private void MySortButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Please specify a valid path.");
                return;
            }

            try
            {
                var sortedItems = myListBox.Items.Cast<string>()
                    .Select(item => new
                    {
                        Name = item,
                        Size = GetDirectorySize(new DirectoryInfo(Path.Combine(path, item)))
                    })
                    .OrderByDescending(item => item.Size)
                    .Select(item => item.Name).ToList();

                if (sortedItems.Count == 0)
                {
                    MessageBox.Show("There are no items to sort.");
                    return;
                }

                myListBox.Items.Clear();
                foreach (var item in sortedItems)
                {
                    myListBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred:. " + ex.Message);
            }
        }

        private long GetDirectorySize(DirectoryInfo dirInfo)
        {
            long size = 0;
            FileInfo[] fileInfos;
            DirectoryInfo[] dirInfos;

            try
            {
                fileInfos = dirInfo.GetFiles();
                dirInfos = dirInfo.GetDirectories();
            }
            catch (UnauthorizedAccessException)
            {
                return size; // Skip directories to which you do not have access rights
            }

            foreach (var fileInfo in fileInfos)
            {
                size += fileInfo.Length;
            }

            foreach (var subDirInfo in dirInfos)
            {
                size += GetDirectorySize(subDirInfo);
            }

            return size;
        }

        private void MyListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = myListBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                string dirPath = Path.Combine(path, myListBox.Items[index].ToString());
                Process.Start(new ProcessStartInfo
                {
                    FileName = dirPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        }

        private void MyDeleteButton_Click(object sender, EventArgs e)
        {
            if (myListBox.SelectedIndex == -1)
            {
                MessageBox.Show("削除する項目を選択してください。");
                return;
            }

            string selectedItem = myListBox.SelectedItem.ToString();
            string fullPath = Path.Combine(path, selectedItem);

            try
            {
                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true); // true for recursive delete
                    myListBox.Items.RemoveAt(myListBox.SelectedIndex);
                }
                else
                {
                    MessageBox.Show("Directory to be deleted is not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while deleting:. " + ex.Message);
            }
        }
    }

    public class b
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
