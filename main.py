from login import login
from checkplu import getPluNumber
from session import SessionHandler
from pwinput import pwinput
from requests import HTTPError, RequestException

name = input('Podaj login: ')
password = pwinput('Podaj hasło: ')

try:
    token, user_id = login(name, password)

    while True:
        session = SessionHandler()
        session.initializeSession(token, user_id, 20)
        items = session.getExecutionItems()
        session.startExecution()

        for item in items:
            plu = getPluNumber(token, item[0])
            print(f'Dla {item[0]} znaleziono PLU {plu}')
            session.sendAnswer(item[2], plu, item[1])

        user_score, max_score = session.getResult()
        print(f'Zakończono sesję. Wynik to {user_score}/{max_score} pkt.')

except RequestException as e:
        print(f'Web request failed: {e}')
except HTTPError as e:
        print(f'HTTP error occurred: {e}')
except ValueError as e:
        print(f'JSON decoding failed: {e}')