from login import login
from checkplu import getPluNumber
from session import *
from pwinput import pwinput
from requests import HTTPError, RequestException

name = input('Podaj login: ')
password = pwinput('Podaj hasło: ')

try:
    token, user_id = login(name, password)

    while True:
        session_id = createNewSession(token, user_id, 20)
        items = getExecutionItems(token, session_id)
        startExecution(token, session_id)

        for item in items:
            plu = getPluNumber(token, item[0])
            sendAnswer(token, item[2], plu, item[1])

        user_score, max_score = getResult(token, session_id, user_id)
        print(f'Zakończono sesję. Wynik to {user_score}/{max_score} pkt.')

except RequestException as e:
        print(f'Web request failed: {e}')
except HTTPError as e:
        print(f'HTTP error occurred: {e}')
except ValueError as e:
        print(f'JSON decoding failed: {e}')