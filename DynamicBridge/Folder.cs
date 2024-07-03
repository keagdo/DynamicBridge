﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBridge;
public class Folder
{
    public string Name = null;
    public string Identifier = "";
    public List<Folder> Subfolders = [];
    public List<FolderItem> Items = [];

    public Folder(string name, List<Folder> subfolders) 
    {
        this.Name = name;
        this.Subfolders = subfolders;
    }

    public void AddItem(string[] path, FolderItem item)
    {
        PluginLog.Information($"Adding with path {path.Print()}");
        if (path.Length == 0)
        {
            Items.Add(item);
        }
        else
        {
            string[] newPath;
            if(path.Length >= 1)
            {
                newPath = path;
            }
            else
            {
                newPath = path[1..^1];
            }
            if(Subfolders.TryGetFirst(x => x.Name == path[0], out var target))
            {
                target.AddItem(newPath, item);
            }
            else
            {
                var newSubfolder = new Folder(path[0], []) { Identifier = path.Join(",") };
                Subfolders.Add(newSubfolder);
                newSubfolder.AddItem(newPath, item);
            }
        }
    }

    public void Draw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.One);
        for (int i = 0; i < Subfolders.Count; i++)
        {
            var item = Subfolders[i];
            if(ImGuiEx.TreeNode(Colors.TabBlue, $"{item.Name}##fldr{Identifier}"))
            {
                item.Draw();
                ImGui.TreePop();
            }
        }
        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            try
            {
                item.Action();
            }
            catch(Exception e)
            {
                ImGuiEx.Text(e.Message);
            }
        }
        ImGui.PopStyleVar();
    }
}
