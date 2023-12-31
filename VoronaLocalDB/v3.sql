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
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id INTEGER NOT NULL UNIQUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP NOT NULL
    --FOREIGN KEY (user_id) REFERENCES Users (id)
);

CREATE TABLE IF NOT EXISTS Messages (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    conversation_id INTEGER NOT NULL,
    content TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
    --reply_to_message_id INTEGER,
    --attachment_id uuid DEFAULT NULL
    --FOREIGN KEY (user_id) REFERENCES Users (id),
    --FOREIGN KEY (conversation_id) REFERENCES Conversations (id)
    --FOREIGN KEY (attachment_id) REFERENCES Attachments (id)
    --FOREIGN KEY (reply_to_message_id) REFERENCES Messages (id)
);

CREATE TABLE IF NOT EXISTS Conversations (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL DEFAULT 'New Conversation',
    type VARCHAR(20) NOT NULL DEFAULT 'Group',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_message_id INTEGER
    --FOREIGN KEY (last_message_id) REFERENCES Messages (id)
    --FOREIGN KEY (message_id) REFERENCES Messages (id)
);

-- Join table betweem users and conversations
CREATE TABLE IF NOT EXISTS User_Conversations (
    user_id INTEGER NOT NULL,
    conversation_id INTEGER NOT NULL,
    PRIMARY KEY (user_id, conversation_id)
    --FOREIGN KEY (user_id) REFERENCES Users (id),
    --FOREIGN KEY (conversation_id) REFERENCES Conversations (id)
);

CREATE TABLE Attachments (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(), 
    message_id INTEGER NOT NULL,  
    --url VARCHAR(255) NOT NULL,
    file BYTEA DEFAULT NULL
    --FOREIGN KEY (message_id) REFERENCES Messages (id)
);

--Add foreign keys to tables
ALTER TABLE Tokens ADD FOREIGN KEY (user_id) REFERENCES Users (id);
ALTER TABLE Messages ADD FOREIGN KEY (user_id) REFERENCES Users (id);
ALTER TABLE Messages ADD FOREIGN KEY (conversation_id) REFERENCES Conversations (id);
--ALTER TABLE Messages ADD FOREIGN KEY (attachment_id) REFERENCES Attachments (id);
ALTER TABLE Conversations ADD FOREIGN KEY (last_message_id) REFERENCES Messages (id);
ALTER TABLE User_Conversations ADD FOREIGN KEY (user_id) REFERENCES Users (id);
ALTER TABLE User_Conversations ADD FOREIGN KEY (conversation_id) REFERENCES Conversations (id);
ALTER TABLE Attachments ADD FOREIGN KEY (message_id) REFERENCES Messages (id);

--Add indexes to tables
CREATE INDEX IF NOT EXISTS idx_user_conversations_user_id ON User_Conversations (user_id);
CREATE INDEX IF NOT EXISTS idx_user_conversations_conversation_id ON User_Conversations (conversation_id);
CREATE INDEX IF NOT EXISTS idx_messages_user_id ON Messages (user_id);
CREATE INDEX IF NOT EXISTS idx_messages_conversation_id ON Messages (conversation_id);
--CREATE INDEX IF NOT EXISTS idx_messages_reply_to_message_id ON Messages (reply_to_message_id);
--CREATE INDEX IF NOT EXISTS idx_messages_attachment_id ON Messages (attachment_id);
CREATE INDEX IF NOT EXISTS idx_attachments_id ON Attachments (id);



-- Functions
CREATE OR REPLACE FUNCTION create_conversation(users_ids integer[], 
                                               conversation_name CHARACTER VARYING, 
                                               conversation_type CHARACTER VARYING) RETURNS void
    LANGUAGE plpgsql
AS
$$
DECLARE
    conversation_id INTEGER;
    user_name VARCHAR;
    user_id INTEGER;
BEGIN
    FOREACH user_id IN ARRAY users_ids LOOP
            SELECT users.username INTO user_name FROM users WHERE users.id = user_id;
            RAISE NOTICE 'User name: %', user_name;
        END LOOP;
    INSERT INTO conversations (name, type) VALUES (conversation_name, conversation_type) RETURNING id INTO conversation_id;
    FOREACH user_id IN ARRAY users_ids LOOP
            INSERT INTO user_conversations (user_id, conversation_id) VALUES (user_id, conversation_id);
        END LOOP;
END
$$;