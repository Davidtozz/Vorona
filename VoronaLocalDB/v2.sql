SET SCHEMA 'public';

/*DROP TABLE user_conversations;
DROP TABLE messages;
DROP TABLE conversations;
DROP TABLE users;*/
/*DROP TABLE "__EFMigrationsHistory";*/

CREATE TABLE Users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(120) NOT NULL,
	refresh_token VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
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
	reply_to_message_id INTEGER,
    FOREIGN KEY (conversation_id) REFERENCES conversations (id),
    FOREIGN KEY (user_id) REFERENCES Users (id),
	FOREIGN KEY (reply_to_message_id) REFERENCES Messages (id)
);

CREATE TABLE User_Conversations (
    user_id INTEGER NOT NULL,
    conversation_id INTEGER NOT NULL,
    PRIMARY KEY (user_id, conversation_id),
    FOREIGN KEY (user_id) REFERENCES Users (id),
    FOREIGN KEY (conversation_id) REFERENCES Conversations (id)
);

CREATE INDEX idx_user_conversations_user_id ON User_Conversations (user_id);
CREATE INDEX idx_messages_conversation_id ON Messages (conversation_id);

