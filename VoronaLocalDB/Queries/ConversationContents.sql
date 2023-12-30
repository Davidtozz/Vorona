SELECT
    Conversations.id AS conversation_id,
    Users.username,
    Messages.content,
    Messages.id
FROM
    Conversations
        INNER JOIN
    User_Conversations ON Conversations.id = User_Conversations.conversation_id
        INNER JOIN
    Users ON User_Conversations.user_id = Users.id
        INNER JOIN
    Messages ON Messages.user_id = Users.id
WHERE
    Conversations.id = 1;