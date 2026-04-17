import sys
import json

def main():
    print("--- Python Script Started ---")
    
    # Check if arguments were passed
    if len(sys.argv) > 1:
        json_str = sys.argv[1]
        print(f"Received JSON string: {json_str}")
        
        try:
            params = json.loads(json_str)
            print(f"Parsed Message: {params.get('message')}")
            print(f"Parsed Value: {params.get('value')}")
            print(f"Is Test: {params.get('isTest')}")
        except Exception as e:
            print(f"Error parsing JSON: {e}")
    else:
        print("No arguments received.")
    
    print("--- Python Script Finished ---")

if __name__ == "__main__":
    main()
