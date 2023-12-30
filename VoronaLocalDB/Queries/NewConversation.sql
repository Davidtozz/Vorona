DROP FUNCTION IF EXISTS create_conversation(integer[], varchar) CASCADE;

create or replace function create_conversation(users_ids integer[], conversation_name character varying) returns void
    language plpgsql
as
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
    INSERT INTO conversations (name) VALUES (conversation_name) RETURNING id INTO conversation_id;
    FOREACH user_id IN ARRAY users_ids LOOP
        INSERT INTO user_conversations (user_id, conversation_id) VALUES (user_id, conversation_id);
    END LOOP;
    END
$$;

alter function create_conversation(integer[], varchar) owner to postgres;

DELETE from conversations CASCADE;

SELECT create_conversation(ARRAY[4,5], 'test');