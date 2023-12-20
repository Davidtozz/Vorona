CREATE TABLE Users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(60) NOT NULL,
    refresh_token VARCHAR(60) NOT NULL,
	salt VARCHAR(30) NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE Conversations (
    id SERIAL PRIMARY KEY,
    type VARCHAR(10) NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE Messages (
    id SERIAL PRIMARY KEY,
    conversation_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    content TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL,
    FOREIGN KEY (conversation_id) REFERENCES conversations (id),
    FOREIGN KEY (user_id) REFERENCES users (id)
);

CREATE TABLE User_Conversations (
    user_id INTEGER NOT NULL,
    conversation_id INTEGER NOT NULL,
    PRIMARY KEY (user_id, conversation_id),
    FOREIGN KEY (user_id) REFERENCES users (id),
    FOREIGN KEY (conversation_id) REFERENCES conversations (id)
);

CREATE INDEX idx_user_conversations_user_id ON user_conversations (user_id);
CREATE INDEX idx_messages_conversation_id ON messages (conversation_id);

