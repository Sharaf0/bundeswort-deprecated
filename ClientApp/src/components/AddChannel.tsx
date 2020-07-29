import React, { useState } from 'react';
import { Form, FormGroup, Label, Input, Button } from 'reactstrap';
import Axios from 'axios';
import swal from 'sweetalert';
import RingLoader from "react-spinners/RingLoader";

export function AddChannel() {
  const [channelUrl, setChannelUrl] = useState<string>('');
  const [language, setLanguage] = useState<string>('DE');
  const [loading, setLoading] = useState<boolean>(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      setLoading(true);
      await Axios.post('https://localhost:5001/api/addchannel/', { channelId: channelUrl, language }, { headers: { 'Content-Type': 'application/json' } });
      swal({ text: 'Channel videos submitted successfully', timer: 5000 });
    } catch (error) {
      swal({ text: 'Some error happened', timer: 5000 });
    } finally {
      setLoading(false);
    }
  }
  return (
    <Form>
      <RingLoader
        size={150}
        color={"#123abc"}
        loading={loading}
      />
      <FormGroup>
        <Label><h1>Channel Id:</h1></Label>
        <Input type="text" placeholder="https://www.youtube.com/channel/UCuhEoi_0aIMKavKnNF_MY3g" value={channelUrl} onChange={(e) => setChannelUrl(e.target.value)} />
        <Label><h1>Language:</h1></Label>
        <Input type="text" placeholder="EN" value={language} onChange={(e) => setLanguage(e.target.value)} />
        <Button type="submit" color="primary"
          onClick={handleSubmit}>Save</Button>{' '}
      </FormGroup>
    </Form>
  );
}
