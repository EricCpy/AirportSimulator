import json
import os
from datetime import datetime

def verwende_custom_date_on_date(custom_date: datetime, input_date: datetime) -> datetime:
    if custom_date:
        input_date.year = custom_date.year
        input_date.month = custom_date.month
        input_date.day = custom_date.day

if __name__ == "__main__":
    data_dir = os.path.join(os.path.dirname(__file__), 'data')

    with open(os.path.join(data_dir, 'arrivals.json') , "r") as arrivals:
        arrival_data = json.load(arrivals)

    with open(os.path.join(data_dir, 'departures.json') , "r") as arrivals:
        departure_data= json.load(arrivals)

    # iteriere durch arrival_data und departure_data und convertiere zu
    # data like {
    #         "arrivals":[
    #            {
    #               "arrivalTime":"11-04-2023 21:52:33",
    #               "airplaneType":"A"
    #            }
    #         ],
    #         "departures":[
    #            {
    #               "departureTime":"11-04-2023 21:52:40",
    #               "airplaneType":"A"
    #            }
    #         ]
    # }

    result = {'arrivals': [], "departures": []}
    custom_date: datetime = None # datetime(.., .., ..) Or None

    for x in arrival_data:
        time = datetime.fromisoformat(x['plannedArrivalTime'].split('[')[0])
        verwende_custom_date_on_date(custom_date, time)
        arrival = {'arrivalTime': str(time), "airplaneType": "Airbus A"}
        result['arrivals'].append(arrival)

    for x in departure_data:
        time = datetime.fromisoformat(x['plannedDepartureTime'].split('[')[0])
        verwende_custom_date_on_date(custom_date, time)
        departure = {'departureTime': str(time), "airplaneType": "Airbus A"}
        result['departures'].append(departure)

    with open(os.path.join(data_dir, 'results.json'), "w") as outfile:
        json.dump(result, outfile)



