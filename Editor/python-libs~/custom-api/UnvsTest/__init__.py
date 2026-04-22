def FirstTest(jon_data:dict)->str:
   
    if(isinstance(jon_data,list)):
        for v in range(len(jon_data)):
            print(jon_data[v])
        
    else:
        for (k,v) in jon_data.items():
            print(k,v)
    
    return "First Test OK"