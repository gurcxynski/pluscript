from login import login
from checkplu import get_plu_number
from session import *

name = input("Podaj login: ")
password = input("Podaj hasło: ")

token, user_id = login(name, password)

while True:

    session_id = create_new_session(token, user_id)

    items = get_execution_items(token, session_id)

    start_execution(token, session_id)

    for item in items:
        print(item)
        plu = get_plu_number(token, item[0])
        print(plu)
        send_answer(token, item[2], plu, item[1])

    result = get_result(token, session_id, user_id)
    print(result)