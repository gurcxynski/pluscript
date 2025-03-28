from pwinput import pwinput
from requests import HTTPError, RequestException
from session import SessionHandler
from login import login
from checkplu import getPluNumber
from getscores import getScoresByCategory, getTotalScore

def printScores(token):
	print(f'Wynik ogólny: {getTotalScore(token)}%\nWyniki w poszczególnych kategoriach:')
	for score, category in getScoresByCategory(token):
		print(f'{category}: {score}%')

logged_in = False

while not logged_in:
	name = input('Podaj login: ')
	password = pwinput('Podaj hasło: ')
	
	try:
		token, user_id = login(name, password)
	except HTTPError as e:
		print('Logowanie nie powiodło się. Spróbuj ponownie.')
		continue
	except ValueError as e:
		print(f'JSON decoding failed: {e}')
		continue
	except RequestException as e:
		print(f'Web request failed: {e}')
		continue
	
	logged_in = True
	print(f'Zalogowano jako {name}.')
	printScores(token)

try:
	while getTotalScore(token) < 100:
		session = SessionHandler()
		session.initializeSession(token, user_id, 20)
		items = session.getExecutionItems()
		session.startExecution()
		print(f'Rozpoczęto sesję. Wybrano {len(items)} produktów.')
		for item in items:
			plu = getPluNumber(token, item[0])
			print(f'Dla {item[0]} znaleziono PLU {plu}')
			session.sendAnswer(item[2], plu, item[1])

		user_score, max_score = session.getResult()
		print(f'Zakończono sesję. Wynik to {user_score}/{max_score} pkt.')
		printScores(token)

except RequestException as e:
	print(f'Web request failed: {e}')
except HTTPError as e:
	print(f'HTTP error occurred: {e}')
except ValueError as e:
	print(f'JSON decoding failed: {e}')

print('Osiągnięto maksymalny wynik!')
input('Naciśnij Enter, aby zakończyć...')