SET SCHEMA 'public';

CREATE TABLE IF NOT EXISTS Users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(120) NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'user',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    settings JSONB NOT NULL DEFAULT '{}'
);

CREATE TABLE Tokens (
    id uuid PRIMARY KEY UNIQUE,
    user_id INTEGER NOT NULL UNIQUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users (id)
);

CREATE TABLE IF NOT EXISTS Messages (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    content TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reply_to_message_id INTEGER,
    -- attachment_id INTEGER,
    FOREIGN KEY (user_id) REFERENCES Users (id),
    FOREIGN KEY (reply_to_message_id) REFERENCES Messages (id)
);

CREATE TABLE IF NOT EXISTS Conversations (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL DEFAULT 'New Conversation',
    type VARCHAR(20) NOT NULL DEFAULT 'group',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    message_id INTEGER,
    last_message_id INTEGER,
    FOREIGN KEY (last_message_id) REFERENCES Messages (id),
    FOREIGN KEY (message_id) REFERENCES Messages (id)
);

-- Join table betweem users and conversations
CREATE TABLE IF NOT EXISTS User_Conversations (
    user_id INTEGER NOT NULL,
    conversation_id INTEGER NOT NULL,
    PRIMARY KEY (user_id, conversation_id),
    FOREIGN KEY (user_id) REFERENCES Users (id),
    FOREIGN KEY (conversation_id) REFERENCES Conversations (id)
);

CREATE TABLE Attachments (
    id SERIAL PRIMARY KEY, 
    message_id INTEGER NOT NULL,  
    --url VARCHAR(255) NOT NULL,
    file BYTEA DEFAULT NULL,
    FOREIGN KEY (message_id) REFERENCES Messages (id)
);

CREATE INDEX IF NOT EXISTS idx_user_conversations_user_id ON User_Conversations (user_id);