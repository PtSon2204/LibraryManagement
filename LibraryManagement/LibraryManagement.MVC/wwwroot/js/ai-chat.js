document.addEventListener("DOMContentLoaded", function () {
    const chatToggle = document.getElementById("ai-chat-toggle");
    const chatWindow = document.getElementById("ai-chat-window");
    const chatClose = document.getElementById("ai-chat-close");
    const chatBody = document.getElementById("ai-chat-body");
    const chatInput = document.getElementById("ai-chat-input");
    const chatSend = document.getElementById("ai-chat-send");

    let chatHistory = [];

    // Toggle Chat Window
    if (chatToggle && chatWindow) {
        chatToggle.addEventListener("click", function () {
            chatWindow.classList.toggle("active");
            if (chatWindow.classList.contains("active")) {
                chatInput.focus();
                scrollToBottom();
            }
        });
    }

    if (chatClose && chatWindow) {
        chatClose.addEventListener("click", function () {
            chatWindow.classList.remove("active");
        });
    }

    // Handle Send Message
    if (chatSend && chatInput) {
        chatSend.addEventListener("click", function () {
            sendMessage();
        });

        chatInput.addEventListener("keydown", function (e) {
            if (e.key === "Enter") {
                sendMessage();
            }
        });
    }

    async function sendMessage() {
        const text = chatInput.value.trim();
        if (!text) return;

        // Clear input
        chatInput.value = "";

        // Add user message to UI
        appendMessage("user", text);

        // Add typing indicator
        const typingIndicator = appendTypingIndicator();
        scrollToBottom();

        try {
            const response = await fetch("/Ai/Chat", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    prompt: text,
                    history: chatHistory
                })
            });

            // Remove typing indicator
            typingIndicator.remove();

            if (response.ok) {
                const data = await response.json();
                const reply = data.response;

                // Add model response to UI
                appendMessage("model", reply);

                // Add to history
                chatHistory.push({ role: "user", content: text });
                chatHistory.push({ role: "model", content: reply });

                // Limit history size to keep payload reasonable (last 10 messages)
                if (chatHistory.length > 20) {
                    chatHistory = chatHistory.slice(-20);
                }
            } else {
                appendMessage("model", "Rất tiếc, đã có lỗi kết nối tới hệ thống AI. Vui lòng thử lại sau!");
            }
        } catch (error) {
            typingIndicator.remove();
            appendMessage("model", "Đã xảy ra sự cố mạng. Vui lòng kiểm tra kết nối!");
            console.error("AI Chat error:", error);
        }

        scrollToBottom();
    }

    function appendMessage(role, text) {
        const msgDiv = document.createElement("div");
        msgDiv.className = `ai-chat-msg ai-chat-msg-${role}`;
        
        const bubble = document.createElement("div");
        bubble.className = "ai-chat-msg-bubble";
        
        if (role === "model") {
            bubble.innerHTML = parseMarkdown(text);
        } else {
            bubble.textContent = text;
        }

        msgDiv.appendChild(bubble);
        chatBody.appendChild(msgDiv);
    }

    function appendTypingIndicator() {
        const indicatorDiv = document.createElement("div");
        indicatorDiv.className = "ai-chat-msg ai-chat-msg-model temp-typing";

        const bubble = document.createElement("div");
        bubble.className = "ai-chat-msg-bubble";

        const indicator = document.createElement("div");
        indicator.className = "ai-typing-indicator";
        indicator.innerHTML = `
            <div class="ai-typing-dot"></div>
            <div class="ai-typing-dot"></div>
            <div class="ai-typing-dot"></div>
        `;

        bubble.appendChild(indicator);
        indicatorDiv.appendChild(bubble);
        chatBody.appendChild(indicatorDiv);
        return indicatorDiv;
    }

    function scrollToBottom() {
        chatBody.scrollTop = chatBody.scrollHeight;
    }

    // Markdown Parser
    function parseMarkdown(text) {
        // Escape HTML
        let escaped = text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");

        // Handle alerts: &gt; [!NOTE] or &gt; [!IMPORTANT]
        escaped = escaped.replace(/&gt;\s*\[!NOTE\]\s*(?:\r?\n)?&gt;\s*(.*)/gi, '<div class="alert alert-info py-2 px-3 mb-2" style="font-size: 0.85rem; border-radius: 8px;"><i class="bi bi-info-circle-fill me-1"></i> $1</div>');
        escaped = escaped.replace(/&gt;\s*\[!IMPORTANT\]\s*(?:\r?\n)?&gt;\s*(.*)/gi, '<div class="alert alert-warning py-2 px-3 mb-2" style="font-size: 0.85rem; border-radius: 8px;"><i class="bi bi-exclamation-triangle-fill me-1"></i> $1</div>');

        // Bold formatting: **text**
        escaped = escaped.replace(/\*\*(.*?)\*\*/g, "<strong>$1</strong>");

        // Inline Code: `code`
        escaped = escaped.replace(/`(.*?)`/g, "<code>$1</code>");

        // Process lists and line breaks line by line
        const lines = escaped.split(/\r?\n/);
        let inList = false;
        const resultLines = [];

        for (let i = 0; i < lines.length; i++) {
            let line = lines[i];
            let trimmed = line.trim();

            if (trimmed.startsWith("- ") || trimmed.startsWith("* ")) {
                if (!inList) {
                    inList = true;
                    resultLines.push('<ul class="ps-3 mb-2">');
                }
                resultLines.push(`<li>${trimmed.substring(2)}</li>`);
            } else {
                if (inList) {
                    inList = false;
                    resultLines.push("</ul>");
                }
                resultLines.push(line);
            }
        }
        if (inList) {
            resultLines.push("</ul>");
        }

        // Join lines with line breaks
        let finalHtml = resultLines.join("\n");
        
        // Replace newlines with <br/> except around block elements
        finalHtml = finalHtml
            .replace(/\n/g, "<br/>")
            .replace(/<br\/><ul/g, "<ul")
            .replace(/<\/ul><br\/>/g, "</ul>")
            .replace(/<br\/><div class="alert/g, '<div class="alert')
            .replace(/<\/div><br\/>/g, "</div>");

        return finalHtml;
    }
});
