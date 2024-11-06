using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class NpcHtmlWindow : L2PopupWindow
{
    private VisualTreeAsset _l2Button;
    private VisualTreeAsset _htmlTable;
    private VisualTreeAsset _htmlRow;
    private VisualTreeAsset _htmlCell;
    private VisualTreeAsset _hyperlink;
    private VisualTreeAsset _htmlWrapper;
    private VisualElement _content;
    private ScrollView _scrollView;
    private static NpcHtmlWindow _instance;
    public static NpcHtmlWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/NpcHtmlWindow");
        _l2Button = LoadAsset("Data/UI/_Elements/Template/L2Button");
        _htmlTable = LoadAsset("Data/UI/_Elements/Template/HtmlTable");
        _htmlRow = LoadAsset("Data/UI/_Elements/Template/HtmlRow");
        _htmlCell = LoadAsset("Data/UI/_Elements/Template/HtmlCell");
        _htmlWrapper = LoadAsset("Data/UI/_Elements/Template/HtmlWrapper");
        _hyperlink = LoadAsset("Data/UI/_Elements/Template/HtmlHyperlink");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
    }
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        Label _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Chat";

        _scrollView = _windowEle.Q<ScrollView>("HtmlContent");
        var _scroller = _scrollView.verticalScroller;
        var highBtn = _scroller.Q<RepeatButton>("unity-high-button");
        var lowBtn = _scroller.Q<RepeatButton>("unity-low-button");
        _content = _windowEle.Q<VisualElement>("ContentWrapper");

        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));

        HideWindow();


        //         RefreshContent(0,
        // @"<html><body>Newbie Helper:<br>
        // Welcome to Einhovant's School of Wizardry. I will be teaching you the basics of combat.<br>
        //  Please click on <font color=""LEVEL"">Quest</font>, in your Chat window.<br>
        // <a action=""bypass -h npc_%objectId%_Quest"">Quest</a><br>
        // </body></html>
        // ", 0);
        //         RefreshContent(0,
        // @"<html><body>Newbie Helper:<br>
        //         Welcome! Are you ready for a mission?<br>
        //         Have you seen the gremlins around here? They've stolen the precious blue gemstone!<br>
        //         <font color=""LEVEL"">You must recover it from them! </font><br>
        //         I'll tell you again how to kill the gremlins. Place your cursor over a gremlin and click the <font color=""FF0000"">left button</font>. The cursor will change to a sword. Click the <font color=""FF0000"">F2 key</font> to attack with <font color=""LEVEL"">Wind Strike</font> magic.<br>
        //         <img src=""L2UI_CH3.tutorial_img12"" width=64 height=64><table border=0><tr><td><img src=""L2UI_CH3.tutorial_img133"" width=64 height=64></td><td><img src=""L2UI_CH3.tutorial_img16"" width=64 height=64></td></tr></table><br>
        //         Complete this mission and I'll reward you with useful items. Good luck!
        //         </body></html>", 0);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("window_open");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow()
    {
        base.HideWindow();
        AudioManager.Instance.PlayUISound("window_close");
        L2GameUI.Instance.WindowClosed(this);
    }

    public void RefreshContent(int npcId, string htmlString, int itemId)
    {
        ShowWindow();

        _content.Clear();

        string processedHtml = PreProcessHtml(htmlString, npcId, itemId);
        ProcessHtmlContent(_content, processedHtml);
    }

    private string PreProcessHtml(string html, int npcId, int itemId)
    {
        // Remove newlines and normalize spaces
        string processed = Regex.Replace(html, @"\s{2,}", "");

        processed = processed
            .Replace("<html>", "")
            .Replace("</html>", "")
            .Replace("<body>", "")
            .Replace("</body>", "")
            .Replace("%objectId%", npcId.ToString())
            .Replace("%item%", itemId.ToString());

        processed = ReplaceItemNames(processed);
        processed = ReplaceSysStrings(processed);
        processed = ReplaceFontColors(processed);

        return processed;
    }

    private string ReplaceItemNames(string processed)
    {
        processed = Regex.Replace(processed, @"\&\#(\d+);", match =>
                {
                    // Extract the captured number
                    string number = match.Groups[1].Value;

                    AbstractItem item = ItemTable.Instance.GetItem(int.Parse(number));
                    if (item != null)
                    {
                        return item.ItemName.Name;
                    }
                    else
                    {
                        return "<Unknown item>";
                    }
                });

        return processed;
    }

    private string ReplaceSysStrings(string processed)
    {
        processed = Regex.Replace(processed, @"\&\$(\d+);", match =>
                {
                    // Extract the captured number
                    string number = match.Groups[1].Value;

                    SysStringData sysString = SysStringTable.Instance.GetSysString(int.Parse(number));
                    if (sysString != null)
                    {
                        return sysString.Name;
                    }
                    else
                    {
                        return "<Unknown SysString>";
                    }
                });

        return processed;
    }


    private string ReplaceFontColors(string processed)
    {
        // Process font color tags while preserving Unity rich text format
        processed = Regex.Replace(
            processed,
            @"<font color=""(.*?)"">(.*?)</font>",
            match =>
            {
                string colorValue = match.Groups[1].Value;
                string content = match.Groups[2].Value;
                return colorValue == "LEVEL"
                    ? $"<color=#FFC900>{content}</color>"
                    : $"<color=#{colorValue}>{content}</color>";
            }
        );

        return processed;
    }

    private void ProcessHtmlContent(VisualElement container, string html)
    {
        List<HtmlNode> nodes = ParseHtmlIntoNodes(html);
        var currentTextBuilder = new StringBuilder();

        foreach (var node in nodes)
        {
            if (node.Type == NodeType.Text)
            {
                currentTextBuilder.Append(node.Content);
            }
            else
            {
                // If we have accumulated text, add it as a text element
                if (currentTextBuilder.Length > 0)
                {
                    AddTextElement(container, currentTextBuilder.ToString());
                    currentTextBuilder.Clear();

                    Debug.LogWarning("CREATE TEXT");
                }
                Debug.LogWarning("CREATE OTHER");
                ProcessNode(container, node);
            }
        }

        // Add any remaining text
        if (currentTextBuilder.Length > 0)
        {
            Debug.LogWarning("CREATE FINAL TEXT");
            AddTextElement(container, currentTextBuilder.ToString());
        }
    }

    private List<HtmlNode> ParseHtmlIntoNodes(string html)
    {
        var nodes = new List<HtmlNode>();
        int currentPos = 0;
        var textBuilder = new StringBuilder();

        while (currentPos < html.Length)
        {
            if (html[currentPos] == '<')
            {
                int tagEnd = html.IndexOf('>', currentPos);
                if (tagEnd == -1) break;

                string tag = html.Substring(currentPos, tagEnd - currentPos + 1);


                if (tag.StartsWith("<br1"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    nodes.Add(new HtmlNode { Type = NodeType.LineBreakSmall });
                    currentPos = tagEnd + 1;
                }
                else if (tag.StartsWith("<br"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    nodes.Add(new HtmlNode { Type = NodeType.LineBreak });
                    currentPos = tagEnd + 1;
                }
                else if (tag.StartsWith("<img"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    var imgMatch = Regex.Match(tag, @"src=""([^""]+)""\s*width=(\d+)\s*height=(\d+)");
                    if (imgMatch.Success)
                    {
                        nodes.Add(new HtmlNode
                        {
                            Type = NodeType.Image,
                            Attributes = new Dictionary<string, string>
                            {
                                ["src"] = imgMatch.Groups[1].Value,
                                ["width"] = imgMatch.Groups[2].Value,
                                ["height"] = imgMatch.Groups[3].Value
                            }
                        });
                    }
                    currentPos = tagEnd + 1;
                }
                else if (tag.StartsWith("<table"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    int tableEnd = html.IndexOf("</table>", currentPos);
                    if (tableEnd != -1)
                    {
                        string tableContent = html.Substring(currentPos, tableEnd - currentPos + 8);
                        nodes.Add(new HtmlNode
                        {
                            Type = NodeType.Table,
                            Content = tableContent
                        });
                        currentPos = tableEnd + 8;
                    }
                    else currentPos = tagEnd + 1;
                }
                else if (tag.StartsWith("<a "))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    int linkEnd = html.IndexOf("</a>", currentPos);
                    if (linkEnd != -1)
                    {
                        string linkTag = html.Substring(currentPos, linkEnd - currentPos);
                        Match actionMatch = Regex.Match(linkTag, @"action=""([^""]+)"".*?>(.*?)$");
                        if (actionMatch.Success)
                        {
                            nodes.Add(new HtmlNode
                            {
                                Type = NodeType.Link,
                                Attributes = new Dictionary<string, string>
                                {
                                    ["action"] = actionMatch.Groups[1].Value.Replace("bypass", "").Replace("-h", "").Trim(),
                                    ["text"] = actionMatch.Groups[2].Value
                                }
                            });
                        }
                        currentPos = linkEnd + 4;
                    }
                    else currentPos = tagEnd + 1;
                }
                else
                {
                    // For <br> and <color> tags, include them in the text content
                    textBuilder.Append(tag);
                    currentPos = tagEnd + 1;
                }
            }
            else
            {
                textBuilder.Append(html[currentPos]);
                currentPos++;
            }
        }

        // Add any remaining text
        if (textBuilder.Length > 0)
        {
            nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
        }

        return nodes;
    }

    private void ProcessNode(VisualElement container, HtmlNode node)
    {
        switch (node.Type)
        {
            case NodeType.Text:
                AddTextElement(container, node.Content);
                break;

            case NodeType.Image:
                AddImageElement(container,
                    node.Attributes["src"],
                    int.Parse(node.Attributes["width"]),
                    int.Parse(node.Attributes["height"])
                );
                break;
            case NodeType.LineBreak:
                AddLineBreak(container, false);
                break;
            case NodeType.LineBreakSmall:
                AddLineBreak(container, true);
                break;
            case NodeType.Table:
                ProcessTable(container, node.Content);
                break;

            case NodeType.Link:
                AddHyperlinkElement(container, node.Attributes["action"], node.Attributes["text"]);
                break;
        }
    }

    private void AddLineBreak(VisualElement container, bool small)
    {
        var spacer = _htmlWrapper.Instantiate()[0];
        if (small)
        {
            spacer.style.height = 0;
        }
        else
        {
            spacer.style.height = 12;
        }
        container.Add(spacer);
    }

    private void AddTextElement(VisualElement container, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        var label = new Label { text = text };
        label.enableRichText = true;
        label.AddToClassList("l2-color-4");
        label.AddToClassList("html-window-label");
        container.Add(label);
    }

    private void AddHyperlinkElement(VisualElement container, string action, string text)
    {
        Button button = (Button)_hyperlink.Instantiate()[0];
        // button.AddToClassList("npc-html-window-button");
        VisualElement buttonLabel = button.Q<Label>("ButtonLabel");
        VisualElement buttonBg = button.Q<VisualElement>("ButtonBg");
        Label label = button.Q<Label>("ButtonLabel");
        label.text = "<u>" + text + "</u>";

        // button.style.marginTop = 15;

        button.clicked += () => ButtonClicked(action);

        container.Add(button);
    }

    private void AddImageElement(VisualElement container, string src, int width, int height)
    {
        var imageContainer = new VisualElement();

        Debug.LogWarning("Image size: " + width + ", " + height);
        imageContainer.style.width = width;
        imageContainer.style.height = height;
        imageContainer.style.flexShrink = new StyleFloat(StyleKeyword.None);

        // if (!_imageCache.ContainsKey(src))
        // {
        //     _imageCache[src] = Resources.Load<Texture2D>(src);
        // }
        Debug.LogWarning("Processing image");
        Texture2D exampleImage = Resources.Load<Texture2D>("Data/SysTextures/Icon/accessary_magic_ring_i00");
        // if (_imageCache[src] != null)

        Debug.Log("Add image element: " + exampleImage);
        if (exampleImage != null)
        {
            var image = new Image();
            // image.image = _imageCache[src];
            image.image = exampleImage;
            image.style.width = width;
            image.style.height = height;
            imageContainer.Add(image);
        }

        container.Add(imageContainer);
    }

    private void ProcessTable(VisualElement container, string tableHtml)
    {
        VisualElement wrapper = _htmlWrapper.Instantiate()[0];

        VisualElement tableContainer = _htmlTable.Instantiate()[0];

        var rows = Regex.Matches(tableHtml, @"<tr>(.*?)</tr>");
        foreach (Match row in rows)
        {
            VisualElement rowElement = _htmlRow.Instantiate()[0];

            var cells = Regex.Matches(row.Groups[1].Value, @"<td>(.*?)</td>");
            foreach (Match cell in cells)
            {
                VisualElement cellElement = _htmlCell.Instantiate()[0];
                string cellContent = cell.Groups[1].Value.Trim();

                // Process cell content recursively

                Debug.LogWarning("Processing table cell content: " + cellContent);

                ProcessHtmlContent(cellElement, cellContent);

                rowElement.Add(cellElement);
            }

            tableContainer.Add(rowElement);
        }

        wrapper.Add(tableContainer);

        container.Add(wrapper);
    }


    private void ButtonClicked(string action)
    {
        Debug.Log(action);
        GameClient.Instance.ClientPacketHandler.RequestBypassToServer(action);
    }

    private enum NodeType
    {
        Text,
        LineBreak,
        LineBreakSmall,
        Image,
        Table,
        Link
    }

    private class HtmlNode
    {
        public NodeType Type { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
